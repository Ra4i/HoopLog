namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTrainingSessionInputModel
    {
        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? DurationMinutes { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
