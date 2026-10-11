namespace HoopLog.ViewModels
{
    using Data.Models.Enums;

    public class TeamDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsCoach { get; set; }

        // Only populated for coaches of the team.
        public string? JoinCode { get; set; }

        public IEnumerable<TeamMemberViewModel> Members { get; set; } = new List<TeamMemberViewModel>();
    }

    public class TeamMemberViewModel
    {
        public string UserId { get; set; } = null!;

        public string DisplayName { get; set; } = null!;

        public TeamRole Role { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
