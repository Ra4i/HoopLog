namespace HoopLog.Data
{
    using HoopLog.Data.Models.Enums;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;
	public class HoopLogDbContext : IdentityDbContext<User>
    {
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
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Drill>()
                .HasIndex(d => d.TeamId);

            modelBuilder.Entity<Drill>(e =>
            {
                e.Property(d => d.Name).HasMaxLength(100);
                e.Property(d => d.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<TrainingSession>(e =>
            {
                e.Property(s => s.Location).HasMaxLength(200);
                e.Property(s => s.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<DrillResult>()
                .Property(dr => dr.Notes)
                .HasMaxLength(500);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainingSession>()
                .HasIndex(s => s.OwnerId);

            modelBuilder.Entity<User>()
                .Property(u => u.DisplayName)
                .HasMaxLength(50)
                .IsRequired();

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

            base.OnModelCreating(modelBuilder);
        }

    }
}