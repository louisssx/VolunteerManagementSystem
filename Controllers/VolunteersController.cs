using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace VolunteerManagementSystem.Controllers
{
    [Authorize]
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor that injects the database context dependency
        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Volunteers
        // Displays a list of all approved volunteers
        // Returns only volunteers where IsApproved is true
        public IActionResult Index()
        {
            var approved = _context.Volunteers.Where(v => v.IsApproved).ToList();
            return View(approved);
        }

        // GET: /Volunteers/Details/5
        // Displays detailed information about a specific volunteer
        // Parameters:
        // - id: The ID of the volunteer to display
        // Returns:
        // - NotFound if id is null or volunteer not found
        // - View with volunteer details and their assigned tasks
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            // Fetch assigned tasks for this volunteer
            var assignedTasks = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Where(t => t.VolunteerId == volunteer.Id)
                .ToList();

            ViewBag.AssignedTasks = assignedTasks;

            return View(volunteer);
        }

        // GET: /Volunteers/Create
        // Displays the form to create a new volunteer
        // Sets up ViewBag with available volunteer types:
        // - Medical
        // - Logistics
        // - Environmental
        public IActionResult Create()
        {
            ViewBag.VolunteerTypes = new SelectList(new List<string>
            {
                "Medical",
                "Logistics",
                "Environmental"
            });

            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Volunteer volunteer)
        {
            volunteer.createdById = "admin";
            volunteer.createdOn = DateTime.Now;

            volunteer.modifiedById = "admin";
            volunteer.modifiedOn = DateTime.Now;

            // Set middleName to '-' if empty
            if (string.IsNullOrWhiteSpace(volunteer.middleName))
            {
                volunteer.middleName = "-";
            }

            if (ModelState.IsValid)
            {
                _context.Add(volunteer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstName,middleName,lastName,phoneNumber,email,address,volunteerType,volunteerSkills,createdById,createdOn,modifiedById,modifiedOn")] Volunteer volunteer)
        {
            if (id != volunteer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.Id == id);
        }

        // GET: /Volunteers/Approved
        // Displays a list of all approved volunteers
        // Returns only volunteers where Status is "Approved"
        public IActionResult Approved()
        {
            var approvedVolunteers = _context.Volunteers
                .Where(v => v.Status == "Approved")
                .ToList();
            return View(approvedVolunteers);
        }

        // GET: /Volunteers/Pending
        // Displays a list of pending and rejected volunteer applications
        // Sets ViewBag.RejectedVolunteers with all rejected volunteers
        // Returns only volunteers where Status is "Pending"
        public IActionResult Pending()
        {
            var pending = _context.Volunteers.Where(v => v.Status == "Pending").ToList();
            var rejected = _context.Volunteers.Where(v => v.Status == "Rejected").ToList();
            ViewBag.RejectedVolunteers = rejected;
            return View(pending);
        }

        // GET: /Volunteers/Rejected
        // Displays a list of all rejected volunteer applications
        // Returns only volunteers where Status is "Rejected"
        public IActionResult Rejected()
        {
            var rejected = _context.Volunteers.Where(v => v.Status == "Rejected").ToList();
            return View(rejected);
        }

        // GET: /Volunteers/Register
        // Displays the volunteer registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Volunteers/Register
        // Handles the submission of the volunteer registration form
        // Parameters:
        // - volunteer: The volunteer data submitted in the form
        // Actions:
        // - Sets middleName to '-' if empty
        // - Sets IsApproved to false
        // - Sets createdOn and modifiedOn to current date/time
        // - Adds the volunteer to the database
        // Returns:
        // - Redirects to RegisterConfirmation on success
        // - Returns to form with validation errors if invalid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Volunteer volunteer)
        {
            // Set middleName to '-' if empty
            if (string.IsNullOrWhiteSpace(volunteer.middleName))
            {
                volunteer.middleName = "-";
            }
            if (ModelState.IsValid)
            {
                volunteer.IsApproved = false;
                volunteer.createdOn = DateTime.Now;
                volunteer.modifiedOn = DateTime.Now;
                _context.Volunteers.Add(volunteer);
                _context.SaveChanges();
                return RedirectToAction("RegisterConfirmation");
            }
            return View(volunteer);
        }

        // GET: /Volunteers/RegisterConfirmation
        // Displays the registration confirmation page after successful registration
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        // POST: /Volunteers/Approve
        // Approves a volunteer application
        // Parameters:
        // - id: The ID of the volunteer to approve
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null && volunteer.Status == "Pending")
            {
                volunteer.Status = "Approved";
                volunteer.IsApproved = true;
                volunteer.modifiedOn = DateTime.Now;
                _context.SaveChanges();

                // Auto-assign this volunteer to the oldest unassigned incident of their type if available
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null && volunteer.Status == "Pending")
            {
                volunteer.Status = "Rejected";
                volunteer.IsApproved = false;
                volunteer.modifiedOn = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Pending");
        }

        [HttpPost]
        public IActionResult Restore(int id)
            {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer != null && volunteer.Status == "Rejected")
            {
                volunteer.Status = "Pending";
                volunteer.IsApproved = false;
                volunteer.modifiedOn = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Pending");
        }
    }
}
