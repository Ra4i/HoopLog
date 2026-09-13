using HoopLog.Data.Models;

namespace HoopLog.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<TrainingSession>> GetSessionsAsync();
        Task<TrainingSession?> GetByIdAsync(int id);
        Task CreateAsync(TrainingSession session);
        Task<bool> UpdateAsync(TrainingSession newSession);
        Task<bool> DeleteAsync(int id);
    }
}