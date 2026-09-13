using HoopLog.Data.Models;

namespace HoopLog.Services
{
    public interface IDrillService
    {
        Task<IEnumerable<Drill>> GetDrillsAsync();
        Task CreateAsync(Drill drill);
    }
}
