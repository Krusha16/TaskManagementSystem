using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    }
}