namespace HoopLog.Services
{
    using Data.Models;

    public interface ISessionService
    {
        Task<IEnumerable<TrainingSession>> GetSessionsAsync();
        Task<TrainingSession?> GetByIdAsync(int id);
        Task CreateAsync(TrainingSession session);
        Task<bool> UpdateAsync(TrainingSession newSession);
        Task<bool> DeleteAsync(int id);
        Task AddDrillResultAsync(DrillResult result);
        Task<DrillResult?> GetDrillResultByIdAsync(int id);
        Task<bool> UpdateDrillResultAsync(DrillResult newResult);
        Task<bool> DeleteDrillResultAsync(int sessionId, int id);
    }
}