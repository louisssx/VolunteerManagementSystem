namespace VolunteerManagementSystem.Models
{
    // Represents a task assigned to a volunteer for a specific incident
    public class TaskAssignment
    {
        public int Id { get; set; } // Unique identifier for the task assignment

        public int IncidentReportId { get; set; } // Foreign key linking to the related incident

        public int VolunteerId { get; set; } // Foreign key linking to the assigned volunteer

        public string TaskDescription { get; set; } // Description of the specific task to perform

        public string Status { get; set; } // Current status of the task (e.g., Assigned, In Progress, Completed)

        public DateTime AssignedDate { get; set; } // Date and time when the task was assigned

        public IncidentReport IncidentReport { get; set; } // Navigation property to the related IncidentReport

        public Volunteer Volunteer { get; set; } // Navigation property to the assigned Volunteer
    }
}
