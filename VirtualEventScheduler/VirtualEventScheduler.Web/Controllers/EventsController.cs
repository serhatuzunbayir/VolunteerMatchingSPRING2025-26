using Microsoft.AspNetCore.Mvc;
using VirtualEventScheduler.Web.Models;
using VirtualEventScheduler.Web.Services;

namespace VirtualEventScheduler.Web.Controllers
{
    /// <summary>
    /// Handles all event-related web pages: listing, details, create, edit, cancel,
    /// participant management, and the current user's registered events.
    /// </summary>
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Helper properties for session-based auth checks
        private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("Token"));
        private string CurrentRole => HttpContext.Session.GetString("UserRole") ?? string.Empty;
        private bool CanManageEvents => CurrentRole == "Admin" || CurrentRole == "Staff";

        /// <summary>Lists all events with optional date/status filters.</summary>
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
                return View(new List<EventViewModel>());
            }
        }

        /// <summary>Shows the detail page for a single event.</summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var eventItem = await _apiService.GetEventByIdAsync(id);
                return View(eventItem);
            }
            catch
            {
                TempData["Error"] = "Event not found";
                return RedirectToAction("Index");
            }
        }

        /// <summary>Registers the current user for an event.</summary>
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

        /// <summary>Unregisters the current user from an event.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int id)
        {
            if (!IsLoggedIn)
                return RedirectToAction("Login", "Account");

            try
            {
                await _apiService.UnregisterFromEventAsync(id);
                TempData["Success"] = "You have been unregistered from the event.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("MyEvents");
        }

        /// <summary>Shows the event creation form (Admin/Staff only).</summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }
            if (!CanManageEvents)
            {
                TempData["Error"] = "Only Admin and Staff users can create events";
                return RedirectToAction(nameof(Index));
            }

            return View(new EventCreateViewModel());
        }

        /// <summary>Handles the event creation form submission.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }
            if (!CanManageEvents)
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

        /// <summary>Shows the event edit form (Admin/Staff only).</summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn || !CanManageEvents)
            {
                TempData["Error"] = "Access denied";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var eventItem = await _apiService.GetEventByIdAsync(id);

                if (eventItem.Status == "Cancelled")
                {
                    TempData["Error"] = "A cancelled event cannot be edited";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new EventUpdateViewModel
                {
                    Id = eventItem.Id,
                    Title = eventItem.Title,
                    Description = eventItem.Description,
                    DateTime = eventItem.DateTime,
                    Location = eventItem.Location,
                    Capacity = eventItem.Capacity,
                    RegisteredCount = eventItem.RegisteredCount
                };

                return View(model);
            }
            catch
            {
                TempData["Error"] = "Event not found";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>Handles the event edit form submission.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventUpdateViewModel model)
        {
            if (!IsLoggedIn || !CanManageEvents)
            {
                TempData["Error"] = "Access denied";
                return RedirectToAction(nameof(Index));
            }

            if (model.DateTime <= DateTime.Now)
                ModelState.AddModelError(nameof(model.DateTime), "Event date must be in the future");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _apiService.UpdateEventAsync(id, model);
                TempData["Success"] = "Event updated successfully";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        /// <summary>Cancels an event (Admin/Staff only). Fires the cancellation delegate in the API.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsLoggedIn || !CanManageEvents)
            {
                TempData["Error"] = "Access denied";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _apiService.CancelEventAsync(id);
                TempData["Success"] = "Event has been cancelled";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// Shows the participant list for an event (Admin/Staff only).
        /// Acts as the "participant tracking" feature for the desktop-equivalent web view.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Participants(int id)
        {
            if (!IsLoggedIn || !CanManageEvents)
            {
                TempData["Error"] = "Access denied";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var eventItem = await _apiService.GetEventByIdAsync(id);
                var participants = await _apiService.GetEventParticipantsAsync(id);

                ViewBag.EventTitle = eventItem.Title;
                ViewBag.EventId = id;

                return View(participants);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Shows all events the current user is registered for, with upcoming reminders highlighted.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
            if (!IsLoggedIn)
            {
                TempData["Error"] = "Please login to view your events";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var myEvents = await _apiService.GetMyEventsAsync();
                return View(myEvents);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<EventViewModel>());
            }
        }
    }
}
