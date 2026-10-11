namespace HoopLog.Services
{
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class SessionService : ISessionService
    {
        private readonly HoopLogDbContext _db;

        public SessionService(HoopLogDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(TrainingSession session, string userId)
        {
            session.OwnerId = userId;
            _db.TrainingSessions.Add(session);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var session = await _db.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == userId);
            if (session is null) return false;
            _db.TrainingSessions.Remove(session);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TrainingSession>> GetSessionsAsync(string userId)
        {
            return await _db.TrainingSessions
                .AsNoTracking()
                .Where(s => s.OwnerId == userId)
                .ToListAsync();
        }

        public async Task<TrainingSession?> GetByIdAsync(int id, string userId)
        {
            return await _db.TrainingSessions
                .Include(s => s.DrillResults)
                .ThenInclude(result => result.Drill)
                .FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == userId);
        }

        public async Task<bool> UpdateAsync(TrainingSession newSession, string userId)
        {
            var sesion = await _db.TrainingSessions
                .FirstOrDefaultAsync(s => s.Id == newSession.Id && s.OwnerId == userId);
            if (sesion is null) return false;
            sesion.Date = newSession.Date;
            sesion.DurationMinutes = newSession.DurationMinutes;
            sesion.Location = newSession.Location;
            sesion.Notes = newSession.Notes;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddDrillResultAsync(DrillResult result, string userId)
        {
            var ownsSession = await _db.TrainingSessions
                .AnyAsync(s => s.Id == result.SessionId && s.OwnerId == userId);
            if (!ownsSession) return false;

            var canUseDrill = await _db.Drills
                .AnyAsync(d => d.Id == result.DrillId && !d.IsArchived && d.TeamId != null &&
                    d.Team!.Memberships.Any(m => m.UserId == userId));
            if (!canUseDrill) return false;

            _db.DrillResults.Add(result);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<DrillResult?> GetDrillResultByIdAsync(int id, string userId)
        {
            return await _db.DrillResults.Include(dr => dr.Drill)
                .FirstOrDefaultAsync(dr => dr.Id == id && dr.TrainingSession.OwnerId == userId);
        }

        public async Task<bool> UpdateDrillResultAsync(DrillResult newResult, string userId)
        {
            var result = await _db.DrillResults
                .FirstOrDefaultAsync(dr => dr.Id == newResult.Id && dr.TrainingSession.OwnerId == userId);
            if (result is null) return false;

            result.Makes = newResult.Makes;
            result.Attempts = newResult.Attempts;
            result.Value = newResult.Value;
            result.Notes = newResult.Notes;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDrillResultAsync(int sessionId, int id, string userId)
        {
            var result = await _db.DrillResults
                .FirstOrDefaultAsync(r => r.Id == id && r.SessionId == sessionId && r.TrainingSession.OwnerId == userId);

            if (result is null) return false;

            _db.DrillResults.Remove(result);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
