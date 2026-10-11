namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CopyStarterDrillInputModel
    {
        [Range(1, int.MaxValue)]
        public int StarterDrillId { get; set; }

        [Range(1, int.MaxValue)]
        public int TeamId { get; set; }
    }
}
