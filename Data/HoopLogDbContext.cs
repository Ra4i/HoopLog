namespace HoopLog.Data
{
    using HoopLog.Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Models;
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

            modelBuilder.Entity<Drill>()
                .Property(d => d.MetricType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(DrillMetricType.MakesAttempts);

            modelBuilder.Entity<Drill>().HasData(
                new Drill { Id = 1, Name = "Free Throws", Category = Category.Shooting, Description = "Shoot free throws from the line, track makes out of attempts.", MetricType = DrillMetricType.MakesAttempts },
                new Drill { Id = 2, Name = "Spot-Up Jumpers", Category = Category.Shooting, Description = "Shoot from five fixed spots around the arc.", MetricType = DrillMetricType.MakesAttempts },
                new Drill { Id = 3, Name = "Cone Dribbling", Category = Category.BallHandling, Description = "Weave through cones using both hands.", MetricType = DrillMetricType.Count },
                new Drill { Id = 4, Name = "Suicides", Category = Category.Conditioning, Description = "Sprint down and back at increasing distances.", MetricType = DrillMetricType.Duration },
                new Drill { Id = 5, Name = "Defensive Slides", Category = Category.Defense, Description = "Lateral slides in a defensive stance.", MetricType = DrillMetricType.Count },
                new Drill { Id = 6, Name = "Core Circuit", Category = Category.Fitness, Description = "Bodyweight strength circuit.", MetricType = DrillMetricType.Count }
            );


            base.OnModelCreating(modelBuilder);
        }

    }
}