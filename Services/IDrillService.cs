namespace HoopLog.Services
{
    using Data.Models;
    using Data.Models.Enums;

    public interface IDrillService
    {
        Task<IEnumerable<Drill>> GetAvailableDrillsAsync(string userId);

        Task<Drill?> GetAvailableByIdAsync(int id, string userId);

        Task<IEnumerable<Drill>> GetManagedDrillsAsync(string userId);

        Task<IEnumerable<Team>> GetCoachedTeamsAsync(string userId);

        Task<Drill?> GetManagedByIdAsync(int id, string userId);

        Task<bool> CreateAsync(Drill drill, int teamId, string userId);

        Task<DrillUpdateResult> UpdateAsync(Drill updatedDrill, string userId);

        Task<bool> ArchiveAsync(int id, string userId);
    }
}
