using System;

namespace VolunteerManagementSystem.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        public int IncidentReportId { get; set; }
        public int VolunteerId { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public IncidentReport IncidentReport { get; set; }
        public Volunteer Volunteer { get; set; }
    }
} 