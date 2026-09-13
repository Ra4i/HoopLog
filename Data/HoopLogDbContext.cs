using Microsoft.EntityFrameworkCore;
using HoopLog.Data.Models;

namespace HoopLog.Data
{
	public class HoopLogDbContext : DbContext
	{
		public HoopLogDbContext() { }

		public HoopLogDbContext(DbContextOptions<HoopLogDbContext> options) : base(options) { }

		public DbSet<Drill> Drills => Set<Drill>();

		public DbSet<DrillResult> DrillResults => Set<DrillResult>();

		public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DrillResult>()
                .HasOne(dr => dr.TrainingSession)
                .WithMany(s => s.DrillResults)
                .HasForeignKey(dr => dr.SessionId)
                .OnDelete(DeleteBehavior.Cascade);   

            modelBuilder.Entity<DrillResult>()
                .HasOne(dr => dr.Drill)
                .WithMany()                          
                .HasForeignKey(dr => dr.DrillId)
                .OnDelete(DeleteBehavior.Restrict);  

            base.OnModelCreating(modelBuilder);
        }

    }
}