namespace HoopLog.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum DrillMetricType
    {
        [Display(Name = "Makes / Attempts")]
        MakesAttempts = 0,

        [Display(Name = "Count (reps)")]
        Count = 1,

        [Display(Name = "Duration (time)")]
        Duration = 2
    }
}
