namespace HoopLog.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Data.Models;
    using ViewModels;
    public class DrillsController : Controller
    {
        private readonly IDrillService _drillService;

        public DrillsController(IDrillService drillService)
        {
            _drillService = drillService;
        }

        public async Task<IActionResult> Index()
        {   
            var drills = await _drillService.GetDrillsAsync();
            return View(drills);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDrillInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var drill = new Drill
            {
                Name = input.Name,
                Category = input.Category,
                Description = input.Description
            };

            await _drillService.CreateAsync(drill);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _drillService.DeleteAsync(id);
            if (!success)
                TempData["Error"] = "This drill can't be deleted — it's used in a training session.";

            return RedirectToAction(nameof(Index));
        }
    }
}
