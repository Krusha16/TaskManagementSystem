using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagementSystem.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsOpened { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? ProjectTaskId { get; set; }
        public virtual ProjectTask ProjectTask { get; set; }
    }
}