using Microsoft.AspNetCore.Mvc;
using VolunteerManagementSystem.Models;
using System.Linq;
using VolunteerManagementSystem.Data;
using System;
using Microsoft.AspNetCore.Authorization;

namespace VolunteerManagementSystem.Controllers
{
    [Authorize]
    public class IncidentReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor that injects the database context dependency
        public IncidentReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /IncidentReports
        // Displays a list of all incident reports
        // Returns a view with the list of reports
        public IActionResult Index()
        {
            return View(_context.IncidentReports.ToList());
        }

        // GET: /IncidentReports/Details/5
        // Displays detailed information about a specific incident report
        // Parameters:
        // - id: The ID of the incident report to display
        // Returns:
        // - NotFound if report not found
        // - View with report details
        public IActionResult Details(int id)
        {
            var report = _context.IncidentReports.FirstOrDefault(r => r.Id == id);
            if (report == null) return NotFound();
            return View(report);
        }

        // GET: /IncidentReports/Create
        // Displays the form to create a new incident report
        // Returns a view with the creation form
        public IActionResult Create()
        {
            return View();
        }

        // POST: /IncidentReports/Create
        // Handles the submission of the incident report form
        // Parameters:
        // - report: The incident report data submitted in the form
        // Returns:
        // - Redirects to Index on success
        // - Returns to form with validation errors if invalid
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

                // Auto-assign one available volunteer of matching type
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

        // GET: IncidentReports/Edit/5
        public IActionResult Edit(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null) return NotFound();
            return View(report);
        }

        // POST: IncidentReports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IncidentReport report)
        {
            if (id != report.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Debug log: print the new status to the console
                Console.WriteLine($"[DEBUG] Updating IncidentReport Id={report.Id}, New Status={report.Status}");
                _context.Update(report);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: IncidentReports/Delete/5
        public IActionResult Delete(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null) return NotFound();
            return View(report);
        }

        // POST: IncidentReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var report = _context.IncidentReports.Find(id);
            if (report == null)
            {
                return NotFound();
            }
            _context.IncidentReports.Remove(report);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
