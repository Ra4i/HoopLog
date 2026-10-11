namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Data.Models.Enums;

    public class EditDrillInputModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(Category))]
        public Category Category { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(DrillMetricType))]
        [Display(Name = "Metric type")]
        public DrillMetricType MetricType { get; set; }
    }
}
