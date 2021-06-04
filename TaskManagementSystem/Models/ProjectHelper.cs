using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagementSystem.Models
{
    public class ProjectHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static Project FindProject(int? id)
        {
            Project project = db.Projects.Find(id);
            return project;
        }

        public static void AddProject(Project project)
        {
            project.Deadline = DateTime.Now;
            db.Projects.Add(project);
            db.SaveChanges();
        }

        

        public static void DeleteProject(int id)
        {
            Project project = FindProject(id);
            db.Projects.Remove(project);
            db.SaveChanges();
        }

        public static void UpdateNotifications(Project project)
        {
            var unFinished = project.ProjectTasks.Where(t => t.IsCompleted == false);
            if (unFinished != null)
            {
                Notification newNotification = new Notification();
                newNotification.ApplicationUserId = project.ApplicationUserId;
                newNotification.Content = project.Name + " : Project is Completed";
                newNotification.ProjectId = project.Id;
                newNotification.DateCreated = DateTime.Now;
                db.Notifications.Add(newNotification);
                project.TotalCost = GetTotalCost(project);
                db.SaveChanges();
            }
        }

        public static double GetTotalCost(Project project)
        {
            HashSet<ApplicationUser> developers = new HashSet<ApplicationUser>();
            foreach (var task in project.ProjectTasks)
            {
                developers.Add(task.ApplicationUser);
            }
            var projectManager = db.Users.Find(project.ApplicationUserId);
            int duration = (Convert.ToDateTime(project.FinishDate) - Convert.ToDateTime(project.StartDate)).Days;
            var dailyCost = developers.Sum(d => d.Salary);
            var totalCost = projectManager.Salary + ((duration) * dailyCost);
            return totalCost;
        }

    }
}