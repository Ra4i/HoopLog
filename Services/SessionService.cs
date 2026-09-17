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

        public async Task CreateAsync(TrainingSession session)
        {
            _db.TrainingSessions.Add(session);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var session = await _db.TrainingSessions.FindAsync(id);
            if (session is null) return false;
            _db.TrainingSessions.Remove(session);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TrainingSession>> GetSessionsAsync()
        {
            return await _db.TrainingSessions.ToListAsync();
        }

        public async Task<TrainingSession?> GetByIdAsync(int id)
        {
            return await _db.TrainingSessions.Include(s => s.DrillResults)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateAsync(TrainingSession newSession)
        {
            var sesion = await _db.TrainingSessions.FindAsync(newSession.Id);
            if (sesion is null) return false;
            sesion.Date = newSession.Date;
            sesion.DurationMinutes = newSession.DurationMinutes;
            sesion.Location = newSession.Location;
            sesion.Notes = newSession.Notes;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task AddDrillResultAsync(DrillResult result)
        {
            _db.DrillResults.Add(result);
            await _db.SaveChangesAsync();
        }

        public async Task<DrillResult?> GetDrillResultByIdAsync(int id)
        {
            return await _db.DrillResults.Include(dr => dr.Drill)
                .FirstOrDefaultAsync(dr => dr.Id == id);
        }

        public async Task<bool> UpdateDrillResultAsync(DrillResult newResult)
        {
            var result = await _db.DrillResults.FindAsync(newResult.Id);
            if (result is null) return false;

            result.Makes = newResult.Makes;
            result.Attempts = newResult.Attempts;
            result.Notes = newResult.Notes;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDrillResultAsync(int sessionId, int id)
        {
            var result = await _db.DrillResults.FirstOrDefaultAsync(r => r.Id == id && r.SessionId == sessionId);

            if (result is null) return false;

            _db.DrillResults.Remove(result);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
