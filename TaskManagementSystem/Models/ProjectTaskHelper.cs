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
    }
}