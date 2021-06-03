using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TaskManagementSystem.Models
{
    public class ProjectTaskHelper
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static ProjectTask FindProjectTask(int? id)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(id);
            return projectTask;
        }

        public static void AddProjectTask(ProjectTask projectTask)
        {
            db.ProjectTasks.Add(projectTask);
            db.SaveChanges();
        }

        public static void DeleteProjectTask(int id)
        {
            ProjectTask projectTask = FindProjectTask(id);
            db.ProjectTasks.Remove(projectTask);
            db.SaveChanges();
        }

        public static void AddDeveloper(int id, string userId)
        {
            ProjectTask projectTask = FindProjectTask(id);
            projectTask.ApplicationUserId = userId;
            db.Entry(projectTask).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static void UpdateNotifications(ApplicationUser applicationUser)
        {
            foreach (var task in applicationUser.ProjectTasks)
            {
                var filteredNotifications = db.Notifications.Where(n => n.ProjectTaskId == task.Id).ToList();
                if (!task.IsCompleted && task.Deadline < DateTime.Now.AddDays(1) && filteredNotifications.Count == 0)
                {
                    Notification newNotification = new Notification();
                    newNotification.ApplicationUserId = applicationUser.Id;
                    newNotification.Content = "Only one day is left to pass the deadline of your task - " + task.Name;
                    newNotification.ProjectTaskId = task.Id;
                    newNotification.DateCreated = DateTime.Now;
                    db.Notifications.Add(newNotification);
                }
            }
            db.SaveChanges();
        }

        public static void UpdateNotificationsForProjects()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            foreach (var project in applicationUser.Projects)
            {
                bool UnFinished = project.ProjectTasks.Any(t => t.IsCompleted == false);
                var filteredNotifications = db.Notifications.Where(n => n.ProjectId == project.Id).ToList();
                if (UnFinished && project.Deadline < DateTime.Now && filteredNotifications.Count == 0)
                {
                    Notification newNotification = new Notification();
                    newNotification.ApplicationUserId = userId;
                    newNotification.Content = project.Name + " : Project passed a deadline with one or more unfinished tasks.";
                    newNotification.ProjectId = project.Id;
                    newNotification.DateCreated = DateTime.Now;
                    db.Notifications.Add(newNotification);
                }
            }
            db.SaveChanges();
        }

        public static void AddUrgentNote(int taskId, string content, string userId, string flag)
        {
            Comment newComment = new Comment();
            newComment.Content = content;
            newComment.ProjectTaskId = taskId;
            newComment.ApplicationUserId = userId;
            if (flag == "Urgent")
                newComment.Flag = Flag.Urgent;
            db.Comments.Add(newComment);
            db.SaveChanges();
        }

        public static void UrgentNotificationToProjectManager(ProjectTask projectTask)
        {
            Notification newNotification = new Notification();
            newNotification.ApplicationUserId = projectTask.Project.ApplicationUserId;
            newNotification.Content = projectTask.Name + " a Task of " + projectTask.Project.Name + " has An Urgent Note to it";
            newNotification.ProjectTaskId = projectTask.Id;
            newNotification.DateCreated = DateTime.Now;
            db.Notifications.Add(newNotification);
            db.SaveChanges();
        }
    }
}