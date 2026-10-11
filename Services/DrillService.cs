namespace HoopLog.Services
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;

    public class DrillService : IDrillService
    {
        private readonly HoopLogDbContext _db;

        public DrillService(HoopLogDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Drill>> GetDrillsAsync()
        {
            return await _db.Drills
                .AsNoTracking()
                .Where(d => !d.IsArchived)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Drill?> GetByIdAsync(int id)
        {
            return await _db.Drills.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Drill>> GetAvailableDrillsAsync(string userId)
        {
            return await _db.Drills
                .AsNoTracking()
                .Include(d => d.Team)
                .Where(d => !d.IsArchived && d.TeamId != null &&
                            d.Team!.Memberships.Any(m => m.UserId == userId))
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Team!.Name)
                .ToListAsync();
        }

        public async Task<Drill?> GetAvailableByIdAsync(int id, string userId)
        {
            return await _db.Drills
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsArchived && d.TeamId != null &&
                    d.Team!.Memberships.Any(m => m.UserId == userId));
        }

        public async Task<IEnumerable<Drill>> GetManagedDrillsAsync(string userId)
        {
            return await _db.Drills
                .AsNoTracking()
                .Include(d => d.Team)
                .Where(d => !d.IsArchived && d.TeamId != null &&
                            d.Team!.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach))
                .OrderBy(d => d.Team!.Name)
                .ThenBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Drill>> GetStarterDrillsAsync()
        {
            return await _db.Drills
                .AsNoTracking()
                .Where(d => d.TeamId == null && !d.IsArchived)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetCoachedTeamsAsync(string userId)
        {
            return await _db.Teams
                .AsNoTracking()
                .Where(t => t.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Drill?> GetManagedByIdAsync(int id, string userId)
        {
            return await _db.Drills
                .AsNoTracking()
                .Include(d => d.Team)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsArchived && d.TeamId != null &&
                    d.Team!.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach));
        }

        public async Task<bool> CreateAsync(Drill drill, int teamId, string userId)
        {
            var coachedTeamId = await _db.Teams
                .Where(t => t.Id == teamId && t.Memberships.Any(m =>
                    m.UserId == userId && m.Role == TeamRole.Coach))
                .Select(t => (int?)t.Id)
                .FirstOrDefaultAsync();
            if (coachedTeamId is null) return false;

            drill.TeamId = coachedTeamId;
            drill.IsArchived = false;
            _db.Drills.Add(drill);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Drill updatedDrill, string userId)
        {
            var drill = await _db.Drills.FirstOrDefaultAsync(d => d.Id == updatedDrill.Id &&
                !d.IsArchived && d.TeamId != null &&
                d.Team!.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach));
            if (drill is null) return false;

            drill.Name = updatedDrill.Name;
            drill.Category = updatedDrill.Category;
            drill.Description = updatedDrill.Description;
            drill.MetricType = updatedDrill.MetricType;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CopyStarterAsync(int starterDrillId, int teamId, string userId)
        {
            var starter = await _db.Drills
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == starterDrillId && d.TeamId == null && !d.IsArchived);
            if (starter is null) return false;

            var coachedTeamId = await _db.Teams
                .Where(t => t.Id == teamId && t.Memberships.Any(m =>
                    m.UserId == userId && m.Role == TeamRole.Coach))
                .Select(t => (int?)t.Id)
                .FirstOrDefaultAsync();
            if (coachedTeamId is null) return false;

            _db.Drills.Add(new Drill
            {
                Name = starter.Name,
                Category = starter.Category,
                Description = starter.Description,
                MetricType = starter.MetricType,
                TeamId = coachedTeamId.Value
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArchiveAsync(int id, string userId)
        {
            var drill = await _db.Drills.FirstOrDefaultAsync(d => d.Id == id &&
                !d.IsArchived && d.TeamId != null &&
                d.Team!.Memberships.Any(m => m.UserId == userId && m.Role == TeamRole.Coach));
            if (drill is null) return false;

            drill.IsArchived = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
