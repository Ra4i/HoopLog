namespace HoopLog.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    public static class EnumDisplayExtensions
    {
        // Uses the enum member's [Display(Name = ...)] when present, otherwise its name.
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
        }
    }
}
