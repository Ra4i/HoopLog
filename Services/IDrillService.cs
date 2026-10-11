namespace HoopLog.Services
{
    using Data.Models;

    public interface IDrillService
    {
        Task<IEnumerable<Drill>> GetDrillsAsync();

        Task<Drill?> GetByIdAsync(int id);

        Task<IEnumerable<Drill>> GetAvailableDrillsAsync(string userId);

        Task<Drill?> GetAvailableByIdAsync(int id, string userId);

        Task<IEnumerable<Drill>> GetManagedDrillsAsync(string userId);

        Task<IEnumerable<Drill>> GetStarterDrillsAsync();

        Task<IEnumerable<Team>> GetCoachedTeamsAsync(string userId);

        Task<Drill?> GetManagedByIdAsync(int id, string userId);

        Task<bool> CreateAsync(Drill drill, int teamId, string userId);

        Task<bool> UpdateAsync(Drill updatedDrill, string userId);

        Task<bool> CopyStarterAsync(int starterDrillId, int teamId, string userId);

        Task<bool> ArchiveAsync(int id, string userId);
    }
}
