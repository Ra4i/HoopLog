namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddDrillResultInputModel : IValidatableObject
    {
        public const int MaxAttempts = 10_000;
        public const int MaxValue = 100_000;

        public int DrillId { get; set; }


        [Range(1, MaxAttempts)]
        public int? Attempts { get; set; }


        [Range(0, MaxAttempts)]
        public int? Makes { get; set; }

        [Range(0, MaxValue)]
        public int? Value { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Makes.HasValue && Attempts.HasValue && Makes > Attempts)
            {
                yield return new ValidationResult("Makes cannot exceed attempts", new[] { nameof(Makes) });
            }
        }
    }
}
