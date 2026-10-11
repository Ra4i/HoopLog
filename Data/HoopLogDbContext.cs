namespace HoopLog.Data
{
    using HoopLog.Data.Models.Enums;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;
	public class HoopLogDbContext : IdentityDbContext<User>
    {
		public HoopLogDbContext() { }

		public HoopLogDbContext(DbContextOptions<HoopLogDbContext> options) : base(options) { }

		public DbSet<Drill> Drills => Set<Drill>();

		public DbSet<DrillResult> DrillResults => Set<DrillResult>();

		public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();

        public DbSet<Team> Teams => Set<Team>();

        public DbSet<TeamMembership> TeamMemberships => Set<TeamMembership>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DrillResult>()
                .HasOne(dr => dr.TrainingSession)
                .WithMany(s => s.DrillResults)
                .HasForeignKey(dr => dr.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DrillResult>()
                .HasOne(dr => dr.Drill)
                .WithMany(d => d.DrillResults)
                .HasForeignKey(dr => dr.DrillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Drill>()
                .Property(d => d.MetricType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(DrillMetricType.MakesAttempts);

            modelBuilder.Entity<Drill>()
                .HasOne(d => d.Team)
                .WithMany()
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Drill>()
                .HasIndex(d => d.TeamId);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainingSession>()
                .HasIndex(s => s.OwnerId);

            modelBuilder.Entity<Team>(e =>
            {
                e.Property(t => t.Name).HasMaxLength(100).IsRequired();
                e.Property(t => t.JoinCode).HasMaxLength(8).IsRequired();
                e.HasIndex(t => t.JoinCode).IsUnique();
            });

            modelBuilder.Entity<TeamMembership>(e =>
            {
                e.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);
                e.HasIndex(m => new { m.TeamId, m.UserId }).IsUnique();   
                e.HasOne(m => m.Team).WithMany(t => t.Memberships)
                    .HasForeignKey(m => m.TeamId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(m => m.User).WithMany()
                    .HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
            });

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