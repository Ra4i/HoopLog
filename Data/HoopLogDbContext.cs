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

            modelBuilder.Entity<Drill>().HasData(
                new Drill { Id = 1, Name = "Free Throws", Category = Models.Enums.Category.Shooting, Description = "Shoot free throws from the line, track makes out of attempts." },
                new Drill { Id = 2, Name = "Spot-Up Jumpers", Category = Models.Enums.Category.Shooting, Description = "Shoot from five fixed spots around the arc." },
                new Drill { Id = 3, Name = "Cone Dribbling", Category = Models.Enums.Category.BallHandling, Description = "Weave through cones using both hands." },
                new Drill { Id = 4, Name = "Suicides", Category = Models.Enums.Category.Conditioning, Description = "Sprint down and back at increasing distances." },
                new Drill { Id = 5, Name = "Defensive Slides", Category = Models.Enums.Category.Defense, Description = "Lateral slides in a defensive stance." },
                new Drill { Id = 6, Name = "Core Circuit", Category = Models.Enums.Category.Fitness, Description = "Bodyweight strength circuit." }
            );


            base.OnModelCreating(modelBuilder);
        }

    }
}