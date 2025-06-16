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

        // Injects the database context
        public VolunteersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Displays approved volunteers
        public IActionResult Index()
        {
            var approved = _context.Volunteers.Where(v => v.IsApproved).ToList();
            return View(approved);
        }

        // Displays volunteer details with assigned tasks
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null) return NotFound();

            var assignedTasks = _context.TaskAssignments
                .Include(t => t.IncidentReport)
                .Where(t => t.VolunteerId == volunteer.Id)
                .ToList();

            ViewBag.AssignedTasks = assignedTasks;

            return View(volunteer);
        }

        // Displays form to create a new volunteer
        public IActionResult Create()
        {
            ViewBag.VolunteerTypes = new SelectList(new List<string> { "Medical", "Logistics", "Environmental" });
            return View();
        }

        // Handles creation of a new volunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Volunteer volunteer)
        {
            volunteer.createdById = "admin";
            volunteer.createdOn = DateTime.Now;
            volunteer.modifiedById = "admin";
            volunteer.modifiedOn = DateTime.Now;

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

        // Displays form to edit volunteer details
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // Handles volunteer detail updates
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstName,middleName,lastName,phoneNumber,email,address,volunteerType,volunteerSkills,createdById,createdOn,modifiedById,modifiedOn")] Volunteer volunteer)
        {
            if (id != volunteer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // Displays delete confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(m => m.Id == id);
            if (volunteer == null) return NotFound();

            return View(volunteer);
        }

        // Confirms and deletes volunteer
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

        // Displays volunteers with "Approved" status
        public IActionResult Approved()
        {
            var approvedVolunteers = _context.Volunteers
                .Where(v => v.Status == "Approved")
                .ToList();
            return View(approvedVolunteers);
        }

        // Displays pending and rejected volunteers
        public IActionResult Pending()
        {
            var pending = _context.Volunteers.Where(v => v.Status == "Pending").ToList();
            var rejected = _context.Volunteers.Where(v => v.Status == "Rejected").ToList();
            ViewBag.RejectedVolunteers = rejected;
            return View(pending);
        }

        // Displays rejected volunteers
        public IActionResult Rejected()
        {
            var rejected = _context.Volunteers.Where(v => v.Status == "Rejected").ToList();
            return View(rejected);
        }

        // Displays registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handles registration submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Volunteer volunteer)
        {
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

        // Shows confirmation after registration
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        // Approves pending volunteer and auto-assigns to incident if available
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

        // Rejects a pending volunteer
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

        // Restores a rejected volunteer to pending status
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
