namespace HoopLog.Controllers
{
    using HoopLog.Data.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Security.Claims;
    using ViewModels;

    public class TeamsController : Controller
    {
        private readonly ITeamService _teams;
        public TeamsController(ITeamService teams) => _teams = teams;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            return View(await _teams.GetMyMembershipsAsync(UserId));
        }

        [HttpGet] 
        public IActionResult Create() 
        {
            return View();
        } 

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamInputModel input)
        {
            if (!ModelState.IsValid) return View(input);
            var team = await _teams.CreateAsync(input.Name, UserId);
            return RedirectToAction(nameof(Details), new { id = team.Id });
        }

        [HttpGet] 
        public IActionResult Join() 
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(JoinTeamInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var result = await _teams.JoinAsync(input.Code, UserId);
            if (result == JoinResult.InvalidCode)
            {
                ModelState.AddModelError(nameof(input.Code), "Invalid or expired code.");
                return View(input);
            }
            if (result == JoinResult.AlreadyMember)
            {
                ModelState.AddModelError(nameof(input.Code), "You're already in this team.");
                return View(input);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var team = await _teams.GetTeamAsync(id, UserId);
            if (team is null) return NotFound();

            return View(team);
        }

        public async Task<IActionResult> Progress(int id)
        {
            var progress = await _teams.GetProgressAsync(id, UserId);
            if (progress is null) return NotFound();

            return View(progress);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RotateCode(int id)
        {
            var code = await _teams.RotateCodeAsync(id, UserId);
            if (code is null) return NotFound();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int id, string userId)
        {
            var removed = await _teams.RemoveMemberAsync(id, userId, UserId);
            if (!removed) return NotFound();
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}