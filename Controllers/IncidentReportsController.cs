using Microsoft.AspNetCore.Mvc;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace VolunteerManagementSystem.Controllers
{
    [Authorize] // Ensures only authenticated users can access this controller
    public class IncidentReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Injects the application's database context
        public IncidentReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays all incident reports
        public IActionResult Index()
        {
            return View(_context.IncidentReports.ToList());
        }

        // Shows details of a specific report by ID
        public IActionResult Details(int id)
        {
            var report = _context.IncidentReports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        // Displays form to create a new incident report
        public IActionResult Create()
        {
            return View();
        }

        // Handles form submission to create a new incident report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IncidentReport report)
        {
            if (ModelState.IsValid)
            {
                report.DateReported = DateTime.Now;
                report.Status = "Pending";
                _context.IncidentReports.Add(report);
                _context.SaveChanges();

                // Auto-assign an available volunteer matching the incident type
                var availableVolunteer = _context.Volunteers
                    .Where(v => v.IsApproved && v.volunteerType.ToLower() == report.Type.ToLower() &&
                        !_context.TaskAssignments.Any(t => t.VolunteerId == v.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                    .FirstOrDefault();

                if (availableVolunteer != null)
                {
                    var assignment = new TaskAssignment
                    {
                        IncidentReportId = report.Id,
                        VolunteerId = availableVolunteer.Id,
                        TaskDescription = $"Assigned to incident: {report.Title}",
                        Status = "Pending",
                        AssignedDate = DateTime.Now
                    };
                    _context.TaskAssignments.Add(assignment);
                    _context.SaveChanges();
                    TempData["AssignedVolunteerName"] = availableVolunteer.firstName + " " + availableVolunteer.lastName;
                }
                else
                {
                    TempData["NoVolunteerType"] = report.Type;
                }

                return Redirect("/IncidentReports/Create#success");
            }
            return View(report);
        }

        // Displays form to edit an existing report
        public IActionResult Edit(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null) return NotFound();
            return View(report);
        }

        // Handles form submission to update a report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IncidentReport report)
        {
            if (id != report.Id) return NotFound();

            if (ModelState.IsValid)
            {
                Console.WriteLine($"[DEBUG] Updating IncidentReport Id={report.Id}, New Status={report.Status}");
                _context.Update(report);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // Displays confirmation page for deleting a report
        public IActionResult Delete(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null) return NotFound();
            return View(report);
        }

        // Handles deletion of a report
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null) return NotFound();

            _context.IncidentReports.Remove(report);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
