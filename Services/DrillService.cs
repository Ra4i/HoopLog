namespace HoopLog.Services
{
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;

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
            return await _db.Drills.OrderByDescending(d => d.Id).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var drill = await _db.Drills.FindAsync(id);
            if (drill is null) return false;

            _db.Drills.Remove(drill);

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Drill?> GetByIdAsync(int id)
        {
            var drill = await _db.Drills.FindAsync(id);
            return drill;
        }
    }
}
