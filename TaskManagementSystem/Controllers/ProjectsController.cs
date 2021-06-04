﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("AllProjects", "Projects");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public ActionResult Create()
        {
            var roleId = db.Roles.Where(r => r.Name == "Project Manager").First().Id;
            var users = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ApplicationUserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                ProjectHelper.AddProject(project);
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", project.ApplicationUserId);
            return View(project);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = ProjectHelper.FindProject(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var roleId = db.Roles.Where(r => r.Name == "Project Manager").First().Id;
            var users = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "Email", project.ApplicationUserId);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ApplicationUserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", project.ApplicationUserId);
            return View(project);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = ProjectHelper.FindProject(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectHelper.DeleteProject(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AllProjects()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            ProjectTaskHelper.UpdateNotificationsForProjects();
            ViewBag.NotificationCount = applicationUser.Notifications.Count;
            var filteredProjects = db.Projects.Where(p => p.ApplicationUserId == userId).ToList();
            var sortedProjects = filteredProjects.OrderByDescending(p => (int)(p.Priority)).ToList();
            return View(sortedProjects);
        }

        public ActionResult SortTasks(string sortBy, int projectId)
        {
            Project project = db.Projects.Find(projectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            var sortedTasks = project.ProjectTasks;

            switch (sortBy)
            {
                case "completetion":
                    sortedTasks = sortedTasks.OrderByDescending(p => p.CompletionPercentage).ToList();
                    break;
                case "hideCompleted":
                    sortedTasks = sortedTasks.Where(t => t.IsCompleted == false)
                        .OrderByDescending(p => p.CompletionPercentage).ToList();
                    break;
                case "priority":
                    sortedTasks = sortedTasks.OrderByDescending(p => (int)(p.Priority)).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            project.ProjectTasks = sortedTasks;
            return View(project);
        }

        public ActionResult SetBudget(int id)
        {
            var project = db.Projects.Find(id);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetBudget(int Id, double budget)
        {
            var project = db.Projects.Find(Id);
            if (project != null)
            {
                project.Budget = budget;
                if (ModelState.IsValid)
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AllProjects");
        }

        public ActionResult SetDeadline(int? id)
        {
            var project = db.Projects.Find(id);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDeadline(int Id, string deadline)
        {
            var date = Convert.ToDateTime(deadline);
            var project = db.Projects.Find(Id);
            if (project != null)
            {
                project.Deadline = date;
                if (ModelState.IsValid)
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AllProjects");
        }

        public ActionResult SetStartDate(int? id)
        {
            var project = db.Projects.Find(id);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetStartDate(int Id, string startDate)
        {
            var date = Convert.ToDateTime(startDate);
            var project = db.Projects.Find(Id);
            if (project != null)
            {
                project.StartDate = date;
                if (ModelState.IsValid)
                {
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AllProjects");
        }

        public ActionResult SetFinishDate(int? id)
        {
            var project = db.Projects.Find(id);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetFinishDate(int Id, DateTime finishDate)
        {
            var date = Convert.ToDateTime(finishDate);
            var project = db.Projects.Find(Id);
            if (project != null)
            {
                project.FinishDate = date;
                if (ModelState.IsValid)
                {
                    foreach (var task in project.ProjectTasks)
                    {
                        task.IsCompleted = true;
                        task.CompletionPercentage = 100;
                        db.Entry(task).State = EntityState.Modified;
                    }
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AllProjects");
        }

        public ActionResult GetProjectsExceededBudget()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var UserProjects = db.Projects.Where(p => p.ApplicationUserId == userId).ToList();
            var ExccededBudgetProjects = new List<Project>();
            foreach (var project in UserProjects)
            {
                var totalCost = ProjectHelper.GetTotalCost(project);
                if (project.Budget < totalCost)
                {
                    project.TotalCost = totalCost;
                    ExccededBudgetProjects.Add(project);
                }
            }
            db.SaveChanges();
            ViewBag.Exceeded = true;
            return View("~/Views/Projects/AllProjects.cshtml", ExccededBudgetProjects);
        }
    }
}
