namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    public class JoinTeamInputModel 
    {
        [Required, StringLength(8, MinimumLength = 8)]
        public string Code { get; set; } = null!;
    }
}
