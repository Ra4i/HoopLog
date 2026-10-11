namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    public class CreateTeamInputModel
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;
    }
}
