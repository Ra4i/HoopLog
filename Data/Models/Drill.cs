namespace HoopLog.Data.Models
{
    using Models.Enums;
    public class Drill
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public Category Category { get; set; }

        public string Description { get; set; } = null!;

        public DrillMetricType MetricType { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        public bool IsArchived { get; set; }

        public ICollection<DrillResult> DrillResults { get; set; } = new List<DrillResult>();
    }
}
