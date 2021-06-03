using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagementSystem.Models
{
    public enum Priority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Priority Priority { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public double Budget { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<ProjectTask> ProjectTasks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public Project()
        {
            this.ProjectTasks = new HashSet<ProjectTask>();
            this.Notifications = new HashSet<Notification>();
        }
    }
}