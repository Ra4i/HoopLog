namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddDrillResultInputModel : IValidatableObject
    {

        public int DrillId { get; set; }


        [Range(1, int.MaxValue)]
        public int? Attempts { get; set; }


        [Range(0, int.MaxValue)]
        public int? Makes { get; set; }

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
