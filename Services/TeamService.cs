namespace HoopLog.Services
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;

    public class TeamService : ITeamService
    {
        // 32 chars, no 0/O/1/I lookalikes
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 8;

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
            await _db.SaveChangesAsync();
            return team;
        }

        public async Task<JoinResult> JoinAsync(string code, string userId)
        {
            var normalized = code.Trim().ToUpperInvariant();
            var team = await _db.Teams.FirstOrDefaultAsync(t => t.JoinCode == normalized);
            if (team is null) return JoinResult.InvalidCode;

            var already = await _db.TeamMemberships
                .AnyAsync(m => m.TeamId == team.Id && m.UserId == userId);
            if (already) return JoinResult.AlreadyMember;

            _db.TeamMemberships.Add(new TeamMembership
            {
                TeamId = team.Id,
                UserId = userId,
                Role = TeamRole.Player,
                JoinedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return JoinResult.Joined;
        }

        public async Task<IEnumerable<TeamMembership>> GetMyMembershipsAsync(string userId) =>
            await _db.TeamMemberships
                .AsNoTracking()
                .Include(m => m.Team)
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Team.Name)
                .ToListAsync();

        public async Task<Team?> GetTeamAsync(int teamId, string userId) =>
            await _db.Teams
                .Include(t => t.Memberships).ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == teamId
                                          && t.Memberships.Any(m => m.UserId == userId));

        public async Task<bool> IsCoachAsync(int teamId, string userId) =>
            await _db.TeamMemberships.AnyAsync(m =>
                m.TeamId == teamId && m.UserId == userId && m.Role == TeamRole.Coach);

        public async Task<string?> RotateCodeAsync(int teamId, string userId)
        {
            if (!await IsCoachAsync(teamId, userId)) return null;

            var team = await _db.Teams.FindAsync(teamId);
            if (team is null) return null;

            team.JoinCode = await GenerateUniqueCodeAsync();
            await _db.SaveChangesAsync();
            return team.JoinCode;
        }

        public async Task<bool> RemoveMemberAsync(int teamId, string memberUserId, string requesterId)
        {
            if (memberUserId == requesterId) return false;            // see edge case below
            if (!await IsCoachAsync(teamId, requesterId)) return false;

            var membership = await _db.TeamMemberships
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == memberUserId);
            if (membership is null) return false;

            _db.TeamMemberships.Remove(membership);
            await _db.SaveChangesAsync();
            return true;
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
