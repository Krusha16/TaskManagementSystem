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
                db.SaveChanges();
            } 
        }
    }
}