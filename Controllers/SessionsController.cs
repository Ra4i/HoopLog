namespace HoopLog.Controllers
{
    using Data.Models;
    using HoopLog.Data.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services;
    using ViewModels;

    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IDrillService _drillService;

        public SessionsController(ISessionService sessionService, IDrillService drillService)
        {
            _sessionService = sessionService;
            _drillService = drillService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionService.GetSessionsAsync();
            return View(sessions);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session is null) return NotFound();

            var input = new CreateTrainingSessionInputModel
            {
                Date = session.Date,
                DurationMinutes = session.DurationMinutes,
                Location = session.Location,
                Notes = session.Notes
            };

            ViewData["SessionId"] = id;
            return View(input);
        }

        [HttpGet]
        public async Task<IActionResult> EditDrillResult(int sessionId, int id)
        {
            var session = await _sessionService.GetByIdAsync(sessionId);
            if (session is null) return NotFound();

            var drillResult = await _sessionService.GetDrillResultByIdAsync(id);
            if (drillResult is null || drillResult.SessionId != sessionId) return NotFound();

            var drill = await _drillService.GetByIdAsync(drillResult.DrillId);
            if (drill is null) return NotFound();

            var input = new EditDrillResultInputModel
            {
                DrillId = drillResult.DrillId,
                MadeShots = drillResult.Makes,
                Attempts = drillResult.Attempts,
                Value = drillResult.Value,
                Notes = drillResult.Notes
            };

            ViewData["SessionId"] = sessionId;
            ViewData["DrillResultId"] = id;
            ViewData["DrillMetricType"] = drill.MetricType.ToString();
            ViewData["DrillName"] = drill.Name;
            ViewData["DrillResultId"] = id;
            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateTrainingSessionInputModel input)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SessionId"] = id;
                return View(input);
            }

            var trainingSession = new TrainingSession
            {
                Id = id,
                Date = input.Date!.Value,
                DurationMinutes = input.DurationMinutes!.Value,
                Location = input.Location,
                Notes = input.Notes
            };

            var success = await _sessionService.UpdateAsync(trainingSession);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainingSessionInputModel input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var trainingSession = new TrainingSession
            {
                Date = input.Date!.Value,
                DurationMinutes = input.DurationMinutes!.Value,
                Location = input.Location,
                Notes = input.Notes
            };

            await _sessionService.CreateAsync(trainingSession);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _sessionService.DeleteAsync(id);
            if (!success)
                TempData["Error"] = "This training session can't be deleted.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDrillResult(int sessionId, int id)
        {
            var session = await _sessionService.GetByIdAsync(sessionId);
            if (session is null) return NotFound();

            var deleted = await _sessionService.DeleteDrillResultAsync(sessionId, id);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDrillResult(int sessionId, int id, EditDrillResultInputModel input)
        {
            var session = await _sessionService.GetByIdAsync(sessionId);
            if (session is null) return NotFound();

            var drill = await _drillService.GetByIdAsync(input.DrillId);
            if (drill is null) return NotFound();

            ValidateResultForMetric(drill.MetricType, input);

            if (!ModelState.IsValid)
            {
                ViewData["SessionId"] = sessionId;
                ViewData["DrillResultId"] = id;
                return View(input);
            }

            var updatedResult = new DrillResult
            {
                Id = id,
                DrillId = input.DrillId,
                Makes = drill.MetricType == DrillMetricType.MakesAttempts ? input.MadeShots : null,
                Attempts = drill.MetricType == DrillMetricType.MakesAttempts ? input.Attempts : null,
                Value = drill.MetricType != DrillMetricType.MakesAttempts ? input.Value : null,
                Notes = input.Notes
            };

            var success = await _sessionService.UpdateDrillResultAsync(updatedResult);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Details), new { id = sessionId });
        }
        private void ValidateResultForMetric(DrillMetricType type, EditDrillResultInputModel input)
        {
            switch (type)
            {
                case DrillMetricType.MakesAttempts:
                    if (input.Attempts is null or <= 0)
                        ModelState.AddModelError(nameof(input.Attempts), "Attempts must be at least 1.");
                    if (input.MadeShots is null or < 0)
                        ModelState.AddModelError(nameof(input.MadeShots), "Makes cannot be negative.");
                    if (input.MadeShots.HasValue && input.Attempts.HasValue && input.MadeShots > input.Attempts)
                        ModelState.AddModelError(nameof(input.MadeShots), "Makes cannot exceed attempts.");
                    break;

                case DrillMetricType.Count:
                case DrillMetricType.Duration:
                    if (input.Value is null or < 0)
                        ModelState.AddModelError(nameof(input.Value),
                            type == DrillMetricType.Duration
                                ? "Duration must be zero or more seconds."
                                : "Count must be zero or more.");
                    break;
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session is null) return NotFound();

            return View(await BuildDetailsViewModel(session));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDrillResult(int sessionId, AddDrillResultInputModel input)
        {
            var session = await _sessionService.GetByIdAsync(sessionId);
            if (session is null) return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Details), await BuildDetailsViewModel(session, input.DrillId));

            var drill = await _drillService.GetByIdAsync(input.DrillId);
            if (drill is null)
            {
                ModelState.AddModelError(nameof(input.DrillId), "Selected drill does not exist.");
                return View(nameof(Details), await BuildDetailsViewModel(session, input.DrillId));
            }

            ValidateResultForMetric(drill.MetricType, input);

            var newResult = new DrillResult
            {
                SessionId = sessionId,
                DrillId = input.DrillId,
                Notes = input.Notes,
                Makes = drill.MetricType == DrillMetricType.MakesAttempts ? input.Makes : null,
                Attempts = drill.MetricType == DrillMetricType.MakesAttempts ? input.Attempts : null,
                Value = drill.MetricType != DrillMetricType.MakesAttempts ? input.Value : null
            };

            await _sessionService.AddDrillResultAsync(newResult);
            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        

        private async Task<SessionDetailsViewModel> BuildDetailsViewModel(TrainingSession session, int selectedDrillId = 0)
        {
            var drills = await _drillService.GetDrillsAsync();

            return new SessionDetailsViewModel
            {
                Session = session,
                DrillId = selectedDrillId,
                AvailableDrillEntities = drills
            };
        }

        private void ValidateResultForMetric(DrillMetricType type, AddDrillResultInputModel input)
        {
            switch (type)
            {
                case DrillMetricType.MakesAttempts:
                    if (input.Attempts is null or <= 0)
                        ModelState.AddModelError(nameof(input.Attempts), "Attempts must be at least 1.");
                    if (input.Makes is null or < 0)
                        ModelState.AddModelError(nameof(input.Makes), "Makes cannot be negative.");
                    if (input.Makes.HasValue && input.Attempts.HasValue && input.Makes > input.Attempts)
                        ModelState.AddModelError(nameof(input.Makes), "Makes cannot exceed attempts.");
                    break;

                case DrillMetricType.Count:
                case DrillMetricType.Duration:
                    if (input.Value is null or < 0)
                        ModelState.AddModelError(nameof(input.Value),
                            type == DrillMetricType.Duration
                                ? "Duration must be zero or more seconds."
                                : "Count must be zero or more.");
                    break;
            }
        }
    }
}
