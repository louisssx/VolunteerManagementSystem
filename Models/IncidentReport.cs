using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models
{
    // Represents an incident report in the system
    public class IncidentReport
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the report

        public string Title { get; set; } // Short title of the incident

        public string Description { get; set; } // Detailed description of the incident

        public DateTime DateReported { get; set; } // Date and time the incident was reported

        public string Status { get; set; } // Current status (e.g., Pending, Resolved)

        [Required]
        public string Type { get; set; } // Required: Incident category (e.g., Medical, Environmental)

        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>(); // Linked tasks for this incident

        public IncidentReport()
        {
            Status = "Pending"; // Default status when created
            TaskAssignments = new List<TaskAssignment>(); // Initialize the collection
        }
    }
}
