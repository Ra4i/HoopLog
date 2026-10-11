namespace HoopLog.Services
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
    using ViewModels;

    public class TeamService : ITeamService
    {
        // 32 chars, no 0/O/1/I lookalikes
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 8;
        private const int MaxCodeAttempts = 3;

        private readonly HoopLogDbContext _db;
        public TeamService(HoopLogDbContext db) => _db = db;

        public async Task<Team> CreateAsync(string name, string userId)
        {
            var team = new Team
            {
                Name = name.Trim(),
                JoinCode = await GenerateUniqueCodeAsync(),
                CreatedAt = DateTime.UtcNow
            };
            team.Memberships.Add(new TeamMembership
            {
                UserId = userId,
                Role = TeamRole.Coach,
                JoinedAt = DateTime.UtcNow
            });

            _db.Teams.Add(team);
            await SaveWithUniqueCodeAsync(team);
            return team;
        }

        public async Task<JoinResult> JoinAsync(string code, string userId)
        {
            var normalized = code.Trim().ToUpperInvariant();
            var team = await _db.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.JoinCode == normalized);
            if (team is null) return JoinResult.InvalidCode;

            var already = await _db.TeamMemberships
                .AnyAsync(m => m.TeamId == team.Id && m.UserId == userId);
            if (already) return JoinResult.AlreadyMember;

            var membership = new TeamMembership
            {
                TeamId = team.Id,
                UserId = userId,
                Role = TeamRole.Player,
                JoinedAt = DateTime.UtcNow
            };
            _db.TeamMemberships.Add(membership);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // The unique (TeamId, UserId) index is the real guard against a concurrent join.
                _db.Entry(membership).State = EntityState.Detached;
                if (await _db.TeamMemberships.AnyAsync(m => m.TeamId == team.Id && m.UserId == userId))
                    return JoinResult.AlreadyMember;
                throw;
            }

            return JoinResult.Joined;
        }

        public async Task<IEnumerable<TeamMembership>> GetMyMembershipsAsync(string userId) =>
            await _db.TeamMemberships
                .AsNoTracking()
                .Include(m => m.Team)
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Team.Name)
                .ToListAsync();

        public async Task<TeamDetailsViewModel?> GetTeamAsync(int teamId, string userId)
        {
            var team = await _db.Teams
                .AsNoTracking()
                .Where(t => t.Id == teamId && t.Memberships.Any(m => m.UserId == userId))
                .Select(t => new TeamDetailsViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsCoach = t.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach),
                    JoinCode = t.JoinCode,
                    Members = t.Memberships
                        .OrderBy(m => m.Role == TeamRole.Coach ? 0 : 1)
                        .ThenBy(m => m.User.DisplayName)
                        .Select(m => new TeamMemberViewModel
                        {
                            UserId = m.UserId,
                            DisplayName = m.User.DisplayName,
                            Role = m.Role,
                            JoinedAt = m.JoinedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (team is { IsCoach: false }) team.JoinCode = null;
            return team;
        }

        public async Task<TeamProgressViewModel?> GetProgressAsync(int teamId, string coachId)
        {
            var teamName = await _db.Teams
                .AsNoTracking()
                .Where(t => t.Id == teamId && t.Memberships.Any(m =>
                    m.UserId == coachId && m.Role == TeamRole.Coach))
                .Select(t => t.Name)
                .FirstOrDefaultAsync();
            if (teamName is null) return null;

            var results = await _db.DrillResults
                .AsNoTracking()
                .Where(r => r.Drill.TeamId == teamId
                    && r.Drill.Team.Memberships.Any(m => m.UserId == coachId && m.Role == TeamRole.Coach)
                    && r.Drill.Team.Memberships.Any(m =>
                        m.UserId == r.TrainingSession.OwnerId && m.Role == TeamRole.Player))
                .OrderBy(r => r.TrainingSession.Owner.DisplayName)
                .ThenByDescending(r => r.TrainingSession.Date)
                .ThenBy(r => r.Drill.Name)
                .Select(r => new PlayerDrillResultViewModel
                {
                    PlayerName = r.TrainingSession.Owner.DisplayName,
                    Date = r.TrainingSession.Date,
                    DrillName = r.Drill.Name,
                    MetricType = r.Drill.MetricType,
                    Makes = r.Makes,
                    Attempts = r.Attempts,
                    Value = r.Value
                })
                .ToListAsync();

            return new TeamProgressViewModel { TeamId = teamId, TeamName = teamName, Results = results };
        }

        public async Task<bool> IsCoachAsync(int teamId, string userId) =>
            await _db.TeamMemberships.AnyAsync(m =>
                m.TeamId == teamId && m.UserId == userId && m.Role == TeamRole.Coach);

        public async Task<string?> RotateCodeAsync(int teamId, string userId)
        {
            if (!await IsCoachAsync(teamId, userId)) return null;

            var team = await _db.Teams.FindAsync(teamId);
            if (team is null) return null;

            team.JoinCode = await GenerateUniqueCodeAsync();
            await SaveWithUniqueCodeAsync(team);
            return team.JoinCode;
        }

        public async Task<bool> RemoveMemberAsync(int teamId, string memberUserId, string requesterId)
        {
            if (memberUserId == requesterId) return false;
            if (!await IsCoachAsync(teamId, requesterId)) return false;

            var membership = await _db.TeamMemberships
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == memberUserId);
            if (membership is null) return false;

            // A team must always keep at least one coach.
            if (membership.Role == TeamRole.Coach &&
                await _db.TeamMemberships.CountAsync(m => m.TeamId == teamId && m.Role == TeamRole.Coach) <= 1)
            {
                return false;
            }

            _db.TeamMemberships.Remove(membership);
            await _db.SaveChangesAsync();
            return true;
        }

        // GenerateUniqueCodeAsync is check-then-insert; the unique JoinCode index is the
        // real guard, so a concurrent collision is retried with a fresh code.
        private async Task SaveWithUniqueCodeAsync(Team team)
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await _db.SaveChangesAsync();
                    return;
                }
                catch (DbUpdateException) when (attempt < MaxCodeAttempts)
                {
                    var codeTaken = await _db.Teams
                        .AnyAsync(t => t.JoinCode == team.JoinCode && t.Id != team.Id);
                    if (!codeTaken) throw;
                    team.JoinCode = await GenerateUniqueCodeAsync();
                }
            }
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            while (true)
            {
                var code = string.Create(CodeLength, 0, (span, _) =>
                {
                    for (var i = 0; i < span.Length; i++)
                        span[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
                });

                if (!await _db.Teams.AnyAsync(t => t.JoinCode == code)) return code;
            }
        }
    }
}
