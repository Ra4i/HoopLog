namespace HoopLog.Controllers
{
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using Services;
    using ViewModels;

    public class SessionsController : Controller
    {
        private const int MaxDurationSeconds = 86_400;

        private readonly ISessionService _sessionService;
        private readonly IDrillService _drillService;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public SessionsController(ISessionService sessionService, IDrillService drillService)
        {
            _sessionService = sessionService;
            _drillService = drillService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionService.GetSessionsAsync(UserId);
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
            var session = await _sessionService.GetByIdAsync(id, UserId);
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
            var drillResult = await _sessionService.GetDrillResultByIdAsync(id, UserId);
            if (drillResult is null || drillResult.SessionId != sessionId) return NotFound();

            var input = new EditDrillResultInputModel
            {
                Makes = drillResult.Makes,
                Attempts = drillResult.Attempts,
                Value = drillResult.Value,
                Notes = drillResult.Notes
            };

            SetEditDrillResultViewData(sessionId, id, drillResult.Drill);
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
                Date = input.Date!.Value.Date,
                DurationMinutes = input.DurationMinutes!.Value,
                Location = input.Location,
                Notes = input.Notes
            };

            var success = await _sessionService.UpdateAsync(trainingSession, UserId);
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
                Date = input.Date!.Value.Date,
                DurationMinutes = input.DurationMinutes!.Value,
                Location = input.Location,
                Notes = input.Notes
            };

            await _sessionService.CreateAsync(trainingSession, UserId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _sessionService.DeleteAsync(id, UserId);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDrillResult(int sessionId, int id)
        {
            var deleted = await _sessionService.DeleteDrillResultAsync(sessionId, id, UserId);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDrillResult(int sessionId, int id, EditDrillResultInputModel input)
        {
            var drillResult = await _sessionService.GetDrillResultByIdAsync(id, UserId);
            if (drillResult is null || drillResult.SessionId != sessionId) return NotFound();

            var metricType = drillResult.Drill.MetricType;
            if (ModelState.IsValid)
                ValidateResultForMetric(metricType, input.Makes, input.Attempts, input.Value);

            if (!ModelState.IsValid)
            {
                SetEditDrillResultViewData(sessionId, id, drillResult.Drill);
                return View(input);
            }

            var updatedResult = new DrillResult
            {
                Id = id,
                SessionId = sessionId,
                Notes = input.Notes,
                Makes = metricType == DrillMetricType.MakesAttempts ? input.Makes : null,
                Attempts = metricType == DrillMetricType.MakesAttempts ? input.Attempts : null,
                Value = metricType != DrillMetricType.MakesAttempts ? input.Value : null
            };

            var success = await _sessionService.UpdateDrillResultAsync(updatedResult, UserId);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _sessionService.GetByIdAsync(id, UserId);
            if (session is null) return NotFound();

            return View(await BuildDetailsViewModel(session));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDrillResult(int sessionId, AddDrillResultInputModel input)
        {
            var session = await _sessionService.GetByIdAsync(sessionId, UserId);
            if (session is null) return NotFound();

            if (!ModelState.IsValid)
                return View(nameof(Details), await BuildDetailsViewModel(session, input.DrillId));

            var drill = await _drillService.GetAvailableByIdAsync(input.DrillId, UserId);
            if (drill is null)
            {
                ModelState.AddModelError(nameof(input.DrillId), "Selected drill is not available.");
                return View(nameof(Details), await BuildDetailsViewModel(session, input.DrillId));
            }

            ValidateResultForMetric(drill.MetricType, input.Makes, input.Attempts, input.Value);
            if (!ModelState.IsValid)
                return View(nameof(Details), await BuildDetailsViewModel(session, input.DrillId));

            var newResult = new DrillResult
            {
                SessionId = sessionId,
                DrillId = input.DrillId,
                Notes = input.Notes,
                Makes = drill.MetricType == DrillMetricType.MakesAttempts ? input.Makes : null,
                Attempts = drill.MetricType == DrillMetricType.MakesAttempts ? input.Attempts : null,
                Value = drill.MetricType != DrillMetricType.MakesAttempts ? input.Value : null
            };

            if (!await _sessionService.AddDrillResultAsync(newResult, UserId)) return NotFound();
            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        private void SetEditDrillResultViewData(int sessionId, int drillResultId, Drill drill)
        {
            ViewData["SessionId"] = sessionId;
            ViewData["DrillResultId"] = drillResultId;
            ViewData["DrillName"] = drill.Name;
            ViewData["MetricType"] = drill.MetricType;
        }

        private async Task<SessionDetailsViewModel> BuildDetailsViewModel(TrainingSession session, int selectedDrillId = 0)
        {
            var drills = await _drillService.GetAvailableDrillsAsync(UserId);

            return new SessionDetailsViewModel
            {
                Session = session,
                DrillId = selectedDrillId,
                AvailableDrillEntities = drills
            };
        }
        private void ValidateResultForMetric(DrillMetricType type, int? makes, int? attempts, int? value)
        {
            switch (type)
            {
                case DrillMetricType.MakesAttempts:
                    if (attempts is null or <= 0)
                        ModelState.AddModelError(nameof(AddDrillResultInputModel.Attempts), "Attempts must be at least 1.");
                    if (makes is null or < 0)
                        ModelState.AddModelError(nameof(AddDrillResultInputModel.Makes), "Makes cannot be negative.");
                    if (makes.HasValue && attempts.HasValue && makes > attempts)
                        ModelState.AddModelError(nameof(AddDrillResultInputModel.Makes), "Makes cannot exceed attempts.");
                    break;

                case DrillMetricType.Count:
                case DrillMetricType.Duration:
                    if (value is null or < 0)
                        ModelState.AddModelError(nameof(AddDrillResultInputModel.Value),
                            type == DrillMetricType.Duration
                                ? "Duration must be zero or more seconds."
                                : "Count must be zero or more.");
                    else if (type == DrillMetricType.Duration && value > MaxDurationSeconds)
                        ModelState.AddModelError(nameof(AddDrillResultInputModel.Value),
                            "Duration cannot be longer than 24 hours (86400 seconds).");
                    break;
            }
        }
    }
}
