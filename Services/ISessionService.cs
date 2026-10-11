namespace HoopLog.Services
{
    using Data.Models;

    public interface ISessionService
    {
        Task<IEnumerable<TrainingSession>> GetSessionsAsync(string userId);
        Task<TrainingSession?> GetByIdAsync(int id, string userId);
        Task CreateAsync(TrainingSession session, string userId);
        Task<bool> UpdateAsync(TrainingSession newSession, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> AddDrillResultAsync(DrillResult result, string userId);
        Task<DrillResult?> GetDrillResultByIdAsync(int id, string userId);
        Task<bool> UpdateDrillResultAsync(DrillResult newResult, string userId);
        Task<bool> DeleteDrillResultAsync(int sessionId, int id, string userId);
    }
}