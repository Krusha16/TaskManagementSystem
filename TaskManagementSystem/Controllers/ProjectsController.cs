using Microsoft.AspNet.Identity;
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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.Projects.Include(p => p.ApplicationUser);
            return View(projects.ToList());
        }

        // GET: Projects/Details/5
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

        // GET: Projects/Create
        [Authorize(Roles = "Project Manager")]
        public ActionResult Create()
        {
            var roleId = db.Roles.Where(r => r.Name == "Project Manager").First().Id;
            var users = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "Email");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ApplicationUserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                /*db.Projects.Add(project);
                db.SaveChanges();*/
                ProjectHelper.AddProject(project);
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", project.ApplicationUserId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*Project project = db.Projects.Find(id);*/
            Project project = ProjectHelper.FindProject(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", project.ApplicationUserId);
            var roleId = db.Roles.Where(r => r.Name == "Project Manager").First().Id;
            var users = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();
            ViewBag.ApplicationUserId = new SelectList(users, "Id", "Email", project.ApplicationUserId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*Project project = db.Projects.Find(id);*/
            Project project = ProjectHelper.FindProject(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Project project = db.Projects.Find(id);
            //db.Projects.Remove(project);
            //db.SaveChanges();
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

        [Authorize(Roles = "Project Manager")]
        public ActionResult AllProjects()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            UpdateNotificationsForProjects();
            ViewBag.NotificationCount = applicationUser.Notifications.Count;
            var filteredProjects = db.Projects.Where(p => p.ApplicationUserId == userId).ToList();
            var sortedProjects = filteredProjects.OrderByDescending(p => (int)(p.Priority)).ToList();
            return View(sortedProjects);
        }

        [Authorize(Roles = "Project Manager")]
        public void UpdateNotificationsForProjects()
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

        [Authorize(Roles = "Project Manager")]
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
    }
}
