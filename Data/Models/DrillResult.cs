namespace HoopLog.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class DrillResult
    {
        public int Id { get; set; }

        [ForeignKey(nameof(TrainingSession))]
        public int SessionId { get; set; }

        public TrainingSession TrainingSession { get; set; } = null!;

        [ForeignKey(nameof(Drill))]
        public int DrillId { get; set; }

        public Drill Drill { get; set; } = null!;

        public int? Makes { get; set; }

        public int? Attempts { get; set; }

        public int? Value { get; set; }

        public string? Notes { get; set; }

       
    }
}
