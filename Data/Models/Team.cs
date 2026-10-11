namespace HoopLog.Data.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string JoinCode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<TeamMembership> Memberships { get; set; }
            = new List<TeamMembership>();
    }
}
