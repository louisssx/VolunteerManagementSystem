using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VolunteerManagementSystem.Models
{
    public class IncidentReport
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateReported { get; set; }
        public string Status { get; set; }
        [Required]
        public string Type { get; set; } // e.g., Medical, Logistics, Environmental, etc.
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

        public IncidentReport()
        {
            Status = "Pending";
            TaskAssignments = new List<TaskAssignment>();
        }
    }
}
