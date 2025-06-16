using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;
using System.Linq;
using System;

namespace VolunteerManagementSystem.Controllers
{
    public class TaskAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context; // Database context for data access

        // Constructor that injects the database context dependency
        public TaskAssignmentsController(ApplicationDbContext context)
        {
            _context = context; // Initialize database context
        }

        // GET: /TaskAssignments
        // Displays a list of all task assignments
        // Includes related IncidentReport and Volunteer data
        // Returns a view with the list of assignments
        public IActionResult Index()
        {
            var assignments = _context.TaskAssignments // Get all task assignments
                .Include(t => t.IncidentReport) // Include related incident report data
                .Include(t => t.Volunteer) // Include related volunteer data
                .ToList();
            return View(assignments); // Return view with assignments list
        }

        // GET: /TaskAssignments/Create
        // Displays the form to create a new task assignment
        // Sets up ViewBag with:
        // - Available incidents that don't have pending/in-progress assignments
        // - Available volunteers who don't have pending/in-progress tasks
        // - Flags indicating if there are no available volunteers/incidents
        public IActionResult Create()
        {
            // Get incidents without pending/in-progress assignments
            var availableIncidents = _context.IncidentReports
                .Where(ir => !_context.TaskAssignments.Any(t => t.IncidentReportId == ir.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.IncidentReports = availableIncidents; // Pass available incidents to view

            // Get volunteers without pending/in-progress tasks
            var availableVolunteers = _context.Volunteers
                .Where(v => !_context.TaskAssignments.Any(t => t.VolunteerId == v.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                .ToList();
            ViewBag.Volunteers = availableVolunteers; // Pass available volunteers to view
            ViewBag.NoAvailableVolunteers = !availableVolunteers.Any(); // Flag if no volunteers available
            ViewBag.NoAvailableIncidents = !availableIncidents.Any(); // Flag if no incidents available
            return View(); // Return creation form view
        }

        // POST: /TaskAssignments/Create
        // Handles the submission of the task assignment form
        // Parameters:
        // - assignment: The task assignment data submitted in the form
        // Returns:
        // - Redirects to Index on success
        // - Returns to form with validation errors if invalid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                // Prevent assignment if incident already has an active assignment
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var assignment = _context.TaskAssignments.Find(id);
            if (assignment != null)
            {
                _context.TaskAssignments.Remove(assignment);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

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
                assignment.IncidentReport.Status = status;
                _context.SaveChanges();

                // Auto-assign this volunteer to the oldest unassigned incident of their type if they are now available
                if (status == "Done")
                {
                    var volunteer = assignment.Volunteer;
                    var unassignedIncident = _context.IncidentReports
                        .Where(ir => ir.Type.ToLower() == volunteer.volunteerType.ToLower() &&
                            !_context.TaskAssignments.Any(t => t.IncidentReportId == ir.Id && (t.Status == "Pending" || t.Status == "In Progress")))
                        .OrderBy(ir => ir.DateReported)
                        .FirstOrDefault();
                    if (unassignedIncident != null)
                    {
                        var newAssignment = new TaskAssignment
                        {
                            IncidentReportId = unassignedIncident.Id,
                            VolunteerId = volunteer.Id,
                            TaskDescription = $"Assigned to incident: {unassignedIncident.Title}",
                            Status = "Pending",
                            AssignedDate = DateTime.Now
                        };
                        _context.TaskAssignments.Add(newAssignment);
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

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