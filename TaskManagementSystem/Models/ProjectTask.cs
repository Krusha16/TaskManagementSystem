using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManagementSystem.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompletionPercentage { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}