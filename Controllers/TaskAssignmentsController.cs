using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers
{
    public class TaskAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context; // Database context

        // Inject the application DB context
        public TaskAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays all task assignments with related incident and volunteer info
        public IActionResult Index()
        {
            var assignments = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Include(t => t.Volunteer)
                .Where(t => t.Status != "Done")
                .ToList();
            return View(assignments);
        }

        // Displays form to manually assign a task to a volunteer
        public IActionResult Create()
        {
            var availableIncidents = _context.IncidentReports
                .Where(ir => !_context.TaskAssignments.Any(t => t.IncidentReportId == ir.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.IncidentReports = availableIncidents;

            var availableVolunteers = _context.Volunteers
                .Where(v => !_context.TaskAssignments.Any(t => t.VolunteerId == v.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.Volunteers = availableVolunteers;

            ViewBag.NoAvailableVolunteers = !availableVolunteers.Any();
            ViewBag.NoAvailableIncidents = !availableIncidents.Any();

            return View();
        }

        // Handles form submission for creating a task assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                // Prevent multiple active assignments for the same incident
                bool alreadyAssigned = _context.TaskAssignments.Any(t => t.IncidentReportId == assignment.IncidentReportId && (t.Status == "Pending" || t.Status == "In Progress"));
                if (alreadyAssigned)
                {
                    TempData["AssignmentError"] = "A volunteer is already assigned to this incident.";
                    return RedirectToAction("Create");
                }

                _context.TaskAssignments.Add(assignment);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, reload data for the view
            var availableIncidents = _context.IncidentReports
                .Where(ir => !_context.TaskAssignments.Any(t => t.IncidentReportId == ir.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.IncidentReports = availableIncidents;

            var availableVolunteers = _context.Volunteers
                .Where(v => !_context.TaskAssignments.Any(t => t.VolunteerId == v.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.Volunteers = availableVolunteers;

            ViewBag.NoAvailableVolunteers = !availableVolunteers.Any();
            ViewBag.NoAvailableIncidents = !availableIncidents.Any();

            return View(assignment);
        }

        // Deletes an assignment based on ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var assignment = _context.TaskAssignments.Find(id);
            if (assignment != null)
            {
                bool wasDone = assignment.Status == "Done";
                _context.TaskAssignments.Remove(assignment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Task assignment deleted successfully.";
                if (wasDone)
                    return RedirectToAction("Done");
            }
            return RedirectToAction(nameof(Index));
        }

        // Updates the status of a task assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            var assignment = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Include(t => t.Volunteer)
                .FirstOrDefault(t => t.Id == id);

            if (assignment != null)
            {
                assignment.Status = status;
                _context.Update(assignment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Task status updated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Displays the form to edit an assignment
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Include(t => t.Volunteer)
                .FirstOrDefault(t => t.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            if (assignment.Status == "Done")
            {
                TempData["EditError"] = "Completed tasks cannot be edited.";
                return RedirectToAction("Index");
            }

            ViewBag.IncidentReports = _context.IncidentReports.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();

            return View(assignment);
        }

        // Displays a list of completed tasks
        public IActionResult Done()
        {
            var doneTasks = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Include(t => t.Volunteer)
                .Where(t => t.Status == "Done")
                .ToList();

            return View(doneTasks);
        }
    }
}
