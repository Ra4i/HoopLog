namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTrainingSessionInputModel : IValidatableObject
    {
        private static readonly DateTime EarliestDate = new(2000, 1, 1);

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes.")]
        public int? DurationMinutes { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // One day of slack: the server's "today" may be behind the user's time zone.
            if (Date > DateTime.Today.AddDays(2))
            {
                yield return new ValidationResult("Date cannot be in the future.", new[] { nameof(Date) });
            }

            if (Date < EarliestDate)
            {
                yield return new ValidationResult("Date must be on or after 1 Jan 2000.", new[] { nameof(Date) });
            }
        }
    }
}
