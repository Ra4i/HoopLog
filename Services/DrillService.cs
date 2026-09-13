using HoopLog.Data;
using HoopLog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HoopLog.Services
{
    public class DrillService : IDrillService
    {
        private readonly HoopLogDbContext _db;

        public DrillService(HoopLogDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Drill drill)
        {
            _db.Drills.Add(drill);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Drill>> GetDrillsAsync()
        {
            return await _db.Drills.ToListAsync();
        }
    }
}
