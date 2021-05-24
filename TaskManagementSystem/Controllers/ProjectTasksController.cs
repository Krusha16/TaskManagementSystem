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
                db.ProjectTasks.Add(projectTask);
                db.SaveChanges();
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
            ProjectTask projectTask = db.ProjectTasks.Find(id);
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
            ProjectTask projectTask = db.ProjectTasks.Find(id);
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
            ProjectTask projectTask = db.ProjectTasks.Find(id);
            db.ProjectTasks.Remove(projectTask);
            db.SaveChanges();
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

        [Authorize(Roles = "Developer")]
        public ActionResult AllTasks()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View(filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
        public ActionResult UpdateCompletedPercetage(int taskId, int percentage)
        {
            ProjectTask projectTask = db.ProjectTasks.Find(taskId);
            if (projectTask == null)
            {
                return HttpNotFound();
            }
            projectTask.CompletionPercentage = percentage;
            if (projectTask.CompletionPercentage >= 100)
            {
                projectTask.CompletionPercentage = 100;
                projectTask.IsCompleted = true;
            }
            db.SaveChanges();
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }

        [Authorize(Roles = "Developer")]
        [HttpPost]
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
            var filteredTasks = db.ProjectTasks.Where(t => t.ApplicationUserId == userId).ToList();
            return View("~/Views/ProjectTasks/AllTasks.cshtml", filteredTasks);
        }
    }
}
