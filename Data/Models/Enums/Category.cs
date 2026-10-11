namespace HoopLog.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum Category
    {
        Shooting = 0,
        [Display(Name = "Ball handling")]
        BallHandling = 1,
        Conditioning = 2,
        Fitness = 3,
        Defense = 4
    }
}
