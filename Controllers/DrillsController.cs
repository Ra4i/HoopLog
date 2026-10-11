namespace HoopLog.Controllers
{
    using System.Security.Claims;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services;
    using ViewModels;

    [Authorize(Policy = "Coach")]
    public class DrillsController : Controller
    {
        private readonly IDrillService _drillService;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public DrillsController(IDrillService drillService)
        {
            _drillService = drillService;
        }

        public async Task<IActionResult> Index()
        {
            var drills = await _drillService.GetManagedDrillsAsync(UserId);
            return View(new DrillIndexViewModel
            {
                TeamGroups = drills
                    .GroupBy(d => d.TeamId)
                    .Select(group => new TeamDrillGroupViewModel
                    {
                        Team = group.First().Team,
                        Drills = group
                    })
                    .ToList()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCoachedTeamsAsync();
            return View(new CreateDrillInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDrillInputModel input)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCoachedTeamsAsync();
                return View(input);
            }

            var drill = new Drill
            {
                Name = input.Name,
                Category = input.Category,
                Description = input.Description,
                MetricType = input.MetricType
            };

            if (!await _drillService.CreateAsync(drill, input.TeamId, UserId)) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var drill = await _drillService.GetManagedByIdAsync(id, UserId);
            if (drill is null) return NotFound();

            ViewData["TeamName"] = drill.Team.Name;
            return View(new EditDrillInputModel
            {
                Name = drill.Name,
                Category = drill.Category,
                Description = drill.Description,
                MetricType = drill.MetricType
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDrillInputModel input)
        {
            var drill = await _drillService.GetManagedByIdAsync(id, UserId);
            if (drill is null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["TeamName"] = drill.Team.Name;
                return View(input);
            }

            var updatedDrill = new Drill
            {
                Id = id,
                Name = input.Name,
                Category = input.Category,
                Description = input.Description,
                MetricType = input.MetricType
            };

            var result = await _drillService.UpdateAsync(updatedDrill, UserId);
            if (result == DrillUpdateResult.NotFound) return NotFound();
            if (result == DrillUpdateResult.MetricTypeLocked)
            {
                ModelState.AddModelError(nameof(input.MetricType),
                    "The metric type can't be changed because players have already logged results for this drill.");
                ViewData["TeamName"] = drill.Team.Name;
                return View(input);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            if (!await _drillService.ArchiveAsync(id, UserId)) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCoachedTeamsAsync()
        {
            var teams = await _drillService.GetCoachedTeamsAsync(UserId);
            ViewData["CoachedTeams"] = new SelectList(teams, "Id", "Name");
        }
    }
}
