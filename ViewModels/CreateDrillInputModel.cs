namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Data.Models.Enums;

    public class CreateDrillInputModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public Category Category { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        public DrillMetricType MetricType { get; set; }
    }
}
