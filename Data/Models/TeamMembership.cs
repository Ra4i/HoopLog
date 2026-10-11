namespace HoopLog.Data.Models
{
    using Enums;
    public class TeamMembership
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public User User { get; set; } = null!;

        public TeamRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
