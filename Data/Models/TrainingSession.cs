namespace HoopLog.Data.Models
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    public class TrainingSession
    {
        
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int DurationMinutes { get; set; }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        [BindNever, ValidateNever]
        public string OwnerId { get; set; } = null!;

        [BindNever, ValidateNever]
        public User Owner { get; set; } = null!;

        public ICollection<DrillResult> DrillResults { get; set; } = new List<DrillResult>();
    }
}
