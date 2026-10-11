namespace HoopLog.ViewModels
{
    using Data.Models.Enums;

    public class TeamProgressViewModel
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = null!;

        public IEnumerable<PlayerDrillResultViewModel> Results { get; set; } = new List<PlayerDrillResultViewModel>();
    }

    // Deliberately limited to drill, date, metric values and display name:
    // session notes, location and result notes are never exposed to coaches.
    public class PlayerDrillResultViewModel
    {
        public string PlayerName { get; set; } = null!;

        public DateTime Date { get; set; }

        public string DrillName { get; set; } = null!;

        public DrillMetricType MetricType { get; set; }

        public int? Makes { get; set; }

        public int? Attempts { get; set; }

        public int? Value { get; set; }
    }
}
