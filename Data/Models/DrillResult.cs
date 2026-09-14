namespace HoopLog.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class DrillResult
    {
        public int Id { get; set; }

        [ForeignKey(nameof(TrainingSession))]
        public int SessionId { get; set; }

        public TrainingSession TrainingSession { get; set; }

        [ForeignKey(nameof(Drill))]
        public int DrillId { get; set; }

        public Drill Drill { get; set; }

        public int? Makes { get; set; }

        public int? Attempts { get; set; }

        public string? Notes { get; set; }

       
    }
}
