namespace HoopLog.Services
{
    using Data.Models;
    using Data.Models.Enums;
    using ViewModels;
    public interface ITeamService
    {
        Task<Team> CreateAsync(string name, string userId);
        Task<JoinResult> JoinAsync(string code, string userId);
        Task<IEnumerable<TeamMembership>> GetMyMembershipsAsync(string userId);
        Task<TeamDetailsViewModel?> GetTeamAsync(int teamId, string userId);
        Task<TeamProgressViewModel?> GetProgressAsync(int teamId, string coachId);
        Task<bool> IsCoachAsync(int teamId, string userId);
        Task<string?> RotateCodeAsync(int teamId, string userId);     
        Task<bool> RemoveMemberAsync(int teamId, string memberUserId, string requesterId);
    }
}
