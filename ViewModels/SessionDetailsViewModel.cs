namespace HoopLog.ViewModels
{
    using Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SessionDetailsViewModel
    {
        public int DrillId { get; set; }

        public TrainingSession Session { get; set; } = null!;

        public IEnumerable<Drill> AvailableDrillEntities { get; set; } = new List<Drill>();
    }
}