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
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Developer, Project Manager")]
        public ActionResult Index()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            return View(applicationUser.Notifications.ToList());
        }

        [Authorize(Roles = "Developer, Project Manager")]
        public ActionResult OpenNotification(int id)
        {
            Notification notification = db.Notifications.Find(id);
            notification.IsOpened = true;
            db.SaveChanges();
            return View(notification);
        }
    }
}
