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

        [Authorize(Roles = "Project Manager,Developer")]
        public ActionResult Index()
        {
            return RedirectToAction("AllTasks", "ProjectTasks");
        }

        [Authorize(Roles = "Project Manager,Developer")]
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

        [Authorize(Roles = "Project Manager")]
        public ActionResult Create()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredProjects = db.Projects.Where(t => t.ApplicationUserId == userId).ToList();
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ProjectId = new SelectList(filteredProjects, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CompletionPercentage,IsCompleted,ApplicationUserId,ProjectId")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                ProjectTaskHelper.AddProjectTask(projectTask);
                return RedirectToAction("AllProjects", "Projects");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Email", projectTask.ApplicationUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectTask.ProjectId);
            return View(projectTask);
        }

        [Authorize(Roles = "Project Manager")]
        public ActionResult Delete(int? id)
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
            return View(projectTask);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectTaskHelper.DeleteProjectTask(id);
            return RedirectToAction("AllProjects", "Projects");
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
            var developers = new List<ApplicationUser>();
            foreach(var user in db.Users)
            {
                if (MembershipHelper.CheckIfUserIsInRole(user.Id, "Developer"))
                {
                    developers.Add(user);
                }
            }
            ViewBag.ApplicationUserId = new SelectList(developers, "Id", "Email");
            return View(projectTask);
        }

        [HttpPost]
        public ActionResult AssignDeveloper(int id, string ApplicationUserId)
        {
            if (ModelState.IsValid)
            {
                ProjectTaskHelper.AddDeveloper(id, ApplicationUserId);
            }
            var developers = new List<ApplicationUser>();
            foreach (var user in db.Users)
            {
                if (MembershipHelper.CheckIfUserIsInRole(user.Id, "Developer"))
                {
                    developers.Add(user);
                }
            }
            ViewBag.ApplicationUserId = new SelectList(developers, "Id", "Email");
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Developer")]
        public ActionResult AllTasks()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            ProjectTaskHelper.UpdateNotifications(applicationUser);
            ViewBag.NotificationCount = applicationUser.Notifications.Where(n => n.IsOpened == false).Count();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View(filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult UpdateCompletedPercentage(int taskId, string percentage)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            projectTask.CompletionPercentage = int.Parse(percentage);
            if (projectTask.CompletionPercentage >= 100)
            {
                projectTask.CompletionPercentage = 100;
                projectTask.IsCompleted = true;
                MembershipHelper.UpdateNotificationsForProjectManager(projectTask);
                ProjectHelper.UpdateNotifications(projectTask.Project);
            }
            else
            {
                projectTask.IsCompleted = false;
            }
            db.SaveChanges();
            ModelState["percentage"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            ViewBag.NotificationCount = applicationUser.Notifications.Where(n => n.IsOpened == false).Count();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult MarkAsCompleted(int taskId)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            if(!projectTask.IsCompleted)
            {
                projectTask.CompletionPercentage = 100;
                projectTask.IsCompleted = true;
            }
            else
            {
                projectTask.IsCompleted = false;
            }
            db.SaveChanges();
            MembershipHelper.UpdateNotificationsForProjectManager(projectTask);
            ProjectHelper.UpdateNotifications(projectTask.Project);
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            ViewBag.NotificationCount = applicationUser.Notifications.Where(n => n.IsOpened == false).Count();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult AddCommentOrUrgentNote(int taskId, string content, string flag)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            ViewBag.NotificationCount = applicationUser.Notifications.Where(n => n.IsOpened == false).Count();
            ProjectTaskHelper.AddUrgentNote(taskId, content, userId, flag);
            if(flag == "Urgent")
                ProjectTaskHelper.UrgentNotificationToProjectManager(projectTask);
            ModelState["content"].Value = new ValueProviderResult("", "", CultureInfo.CurrentCulture);
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
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

        [Authorize(Roles = "Project Manager")]
        public ActionResult UpdateDeadline(int? Id)
        {
            var projectTask = db.ProjectTasks.Find(Id);
            return View(projectTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDeadline(int Id, string deadline)
        {
            var projectTask = db.ProjectTasks.Find(Id);
            if (projectTask != null)
            {
                projectTask.Deadline = Convert.ToDateTime(deadline);
                db.SaveChanges();
            }
            return RedirectToAction("AllProjects", "Projects");
        }

        [Authorize(Roles = "Project Manager")]
        public ActionResult UpdatePriority(int? id)
        {
            var projectTask = db.ProjectTasks.Find(id);
            return View(projectTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePriority(int Id, Priority priority)
        {
            var projectTask = db.ProjectTasks.Find(Id);
            if (projectTask != null)
            {
                projectTask.Priority = priority;
                if (ModelState.IsValid)
                {
                    db.Entry(projectTask).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AllProjects", "Projects");
        }
    }
}
