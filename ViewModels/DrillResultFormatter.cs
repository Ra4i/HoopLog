namespace HoopLog.ViewModels
{
    using Data.Models.Enums;

    public static class DrillResultFormatter
    {
        public static string Format(DrillMetricType metricType, int? makes, int? attempts, int? value)
        {
            return metricType switch
            {
                DrillMetricType.MakesAttempts when makes.HasValue && attempts is > 0
                    => $"{makes}/{attempts} ({(double)makes.Value / attempts.Value:P0})",
                DrillMetricType.MakesAttempts when makes.HasValue || attempts.HasValue
                    => $"{makes?.ToString() ?? "-"}/{attempts?.ToString() ?? "-"}",
                DrillMetricType.Count when value.HasValue
                    => $"{value} reps",
                DrillMetricType.Duration when value.HasValue
                    => TimeSpan.FromSeconds(value.Value).ToString(@"m\:ss"),
                _ => "—"
            };
        }
    }
}
