namespace HoopLog.Authorization
{
    using System.Security.Claims;
    using Data;
    using Data.Models.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

    public sealed class CoachRequirementHandler : AuthorizationHandler<CoachRequirement>
    {
        private readonly HoopLogDbContext _db;

        public CoachRequirementHandler(HoopLogDbContext db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CoachRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId is null) return;

            if (await _db.TeamMemberships.AnyAsync(m =>
                    m.UserId == userId && m.Role == TeamRole.Coach))
            {
                context.Succeed(requirement);
            }
        }
    }
}
