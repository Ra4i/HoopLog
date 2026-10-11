namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterInputModel
    {
        private string _name = null!;

        // Trimmed on binding so the length rule applies to the stored value.
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim()!;
        }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
