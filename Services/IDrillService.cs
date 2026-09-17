namespace HoopLog.Services
{
    using Data.Models;

    public interface IDrillService
    {
        Task<IEnumerable<Drill>> GetDrillsAsync();

        Task CreateAsync(Drill drill);

        Task<bool> DeleteAsync(int id);

        Task<Drill?> GetByIdAsync(int id);
    }
}
