namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class EditDrillResultInputModel : IValidatableObject
    {
        [Range(0, AddDrillResultInputModel.MaxAttempts)]
        public int? Makes { get; set; }

        [Range(1, AddDrillResultInputModel.MaxAttempts)]
        public int? Attempts { get; set; }

        [Range(0, AddDrillResultInputModel.MaxValue)]
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
