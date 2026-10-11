namespace HoopLog.ViewModels
{
    using Data.Models;

    public class DrillIndexViewModel
    {
        public IEnumerable<TeamDrillGroupViewModel> TeamGroups { get; set; } = new List<TeamDrillGroupViewModel>();
    }

    public class TeamDrillGroupViewModel
    {
        public Team Team { get; set; } = null!;

        public IEnumerable<Drill> Drills { get; set; } = new List<Drill>();
    }
}
