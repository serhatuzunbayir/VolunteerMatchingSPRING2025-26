using Microsoft.AspNetCore.Mvc;
using VirtualEventScheduler.Web.Models;
using VirtualEventScheduler.Web.Services;

namespace VirtualEventScheduler.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("Token"));
        private string CurrentRole => HttpContext.Session.GetString("UserRole") ?? string.Empty;
        private bool CanCreateEvents => CurrentRole == "Admin" || CurrentRole == "Staff";

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string? status)
        {
            try
            {
                var events = await _apiService.GetEventsAsync(startDate, endDate, status);
                
                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
                ViewBag.Status = status;
                
                return View(events);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading events: {ex.Message}";
                return View(new List<Models.EventViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var eventItem = await _apiService.GetEventByIdAsync(id);
                return View(eventItem);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Event not found";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id)
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login to register for events";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _apiService.RegisterForEventAsync(id);
                TempData["Success"] = "Successfully registered for the event!";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }
            if (!CanCreateEvents)
            {
                TempData["Error"] = "Only Admin and Staff users can create events";
                return RedirectToAction(nameof(Index));
            }

            return View(new EventCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }
            if (!CanCreateEvents)
            {
                TempData["Error"] = "Only Admin and Staff users can create events";
                return RedirectToAction(nameof(Index));
            }

            if (model.DateTime <= DateTime.Now)
                ModelState.AddModelError(nameof(model.DateTime), "Event date must be in the future");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var created = await _apiService.CreateEventAsync(model);
                TempData["Success"] = $"Event \"{created.Title}\" created successfully";
                return RedirectToAction(nameof(Details), new { id = created.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
