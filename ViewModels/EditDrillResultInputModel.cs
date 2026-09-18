namespace HoopLog.ViewModels
{
    public class EditDrillResultInputModel
    {
        public int DrillId { get; set; }

        public int? MadeShots { get; set; }

        public int? Attempts { get; set; }

        public int? Value { get; set; }

        public string? Notes { get; set; }
    }
}