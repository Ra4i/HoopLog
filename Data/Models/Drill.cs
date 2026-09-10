namespace HoopLog.Data.Models
{
    using Models.Enums;
    public class Drill
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public Category Category { get; set; }

        public string Description { get; set; } = null!;

        public ICollection<DrillResult> DrillResults { get; set; } = new List<DrillResult>();
    }
}
