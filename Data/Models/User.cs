namespace HoopLog.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    public class User : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}
