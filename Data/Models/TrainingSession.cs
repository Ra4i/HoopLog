namespace HoopLog.Data.Models
{
    public class TrainingSession
    {
        
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int DurationMinutes { get; set; }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        public ICollection<DrillResult> DrillResults { get; set; } = new List<DrillResult>();
    }
}
