using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class ProjectTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectTasks
        public ActionResult Index()
        {
            var projectTasks = db.ProjectTasks.Include(p => p.ApplicationUser).Include(p => p.Project);
            return View(projectTasks.ToList());
        }

        // GET: ProjectTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectTask projectTask = db.ProjectTasks.Find(id);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            return View(projectTask);
        }

        // GET: ProjectTasks/Create
        public ActionResult Create()
        {
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        // POST: ProjectTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CompletionPercentage,IsCompleted,ApplicationUserId,ProjectId")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                /*db.ProjectTasks.Add(projectTask);
                db.SaveChanges();*/
                ProjectTaskHelper.AddProjectTask(projectTask);
                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", projectTask.ApplicationUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectTask.ProjectId);
            return View(projectTask);
        }

        // GET: ProjectTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*ProjectTask projectTask = db.ProjectTasks.Find(id);*/
            ProjectTask projectTask = ProjectTaskHelper.FindProjectTask(id);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", projectTask.ApplicationUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectTask.ProjectId);
            return View(projectTask);
        }

        // POST: ProjectTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CompletionPercentage,IsCompleted,ApplicationUserId,ProjectId")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", projectTask.ApplicationUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectTask.ProjectId);
            return View(projectTask);
        }

        // GET: ProjectTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*ProjectTask projectTask = db.ProjectTasks.Find(id);*/
            ProjectTask projectTask = ProjectTaskHelper.FindProjectTask(id);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            return View(projectTask);
        }

        // POST: ProjectTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //ProjectTask projectTask = db.ProjectTasks.Find(id);
            //db.ProjectTasks.Remove(projectTask);
            //db.SaveChanges();
            ProjectTaskHelper.DeleteProjectTask(id);
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

        public ActionResult AssignDeveloper(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectTask projectTask = ProjectTaskHelper.FindProjectTask(id);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            return View(projectTask);
        }

        [HttpPost]
        public ActionResult AssignDeveloper(int id, string ApplicationUserId)
        {
            if (ModelState.IsValid)
            {
                ProjectTaskHelper.AddDeveloper(id, ApplicationUserId);
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Developer")]
        public ActionResult AllTasks()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            UpdateNotifications();
            ViewBag.NotificationCount = applicationUser.Notifications.Count;
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View(filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        public void UpdateNotifications()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            foreach(var task in applicationUser.ProjectTasks)
            {
                if(!task.IsCompleted && task.Deadline < DateTime.Now.AddDays(1))
                {
                    Notification newNotification = new Notification();
                    newNotification.ApplicationUserId = userId;
                    newNotification.Content = "Only one day is left to pass the deadline of your task - " + task.Name;
                    newNotification.ProjectTaskId = task.Id;
                    newNotification.DateCreated = DateTime.Now;
                    var filteredNotifications = db.Notifications.Where(n => n.ProjectTaskId == task.Id);
                    if(filteredNotifications == null)
                    {
                        db.Notifications.Add(newNotification);
                    }
                }
            }
            db.SaveChanges();
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult UpdateCompletedPercentage(int taskId, string percentage)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            projectTask.CompletionPercentage = int.Parse(percentage);
            if (projectTask.CompletionPercentage >= 100)
            {
                projectTask.CompletionPercentage = 100;
                projectTask.IsCompleted = true;
            }
            db.SaveChanges();
            ModelState["percentage"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult MarkAsCompleted(int taskId)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            projectTask.CompletionPercentage = 100;
            projectTask.IsCompleted = true;
            db.SaveChanges();
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult AddComment(int taskId, string content)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            if (projectTask.IsCompleted)
            {
                Comment newComment = new Comment();
                newComment.Content = content;
                newComment.ProjectTaskId = taskId;
                newComment.ApplicationUserId = userId;
                db.Comments.Add(newComment);
            }
            db.SaveChanges();
            ModelState["content"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult AllNotifications()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            return View(applicationUser.Notifications.ToList());
        }

        [Authorize(Roles = "Project Manager")]
        public ActionResult UnFinishedPassedDeadline()
        {
            List<ProjectTask> sortedTasks = new List<ProjectTask>();
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredProjects = db.Projects.Where(p => p.ApplicationUserId == userId).ToList();
            foreach (var project in filteredProjects)
            {
                foreach(var task in project.ProjectTasks)
                {
                    if(!task.IsCompleted && task.Deadline < DateTime.Now)
                    {
                        sortedTasks.Add(task);
                    }
                }
            }
            return View(sortedTasks);
        }
    }
}
