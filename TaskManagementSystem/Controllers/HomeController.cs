using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if(userId != null)
            {
                if (MembershipHelper.CheckIfUserIsInRole(userId, "Project Manager"))
                {
                    return RedirectToAction("AllProjects", "Projects");
                }
                else if (MembershipHelper.CheckIfUserIsInRole(userId, "Developer"))
                {
                    return RedirectToAction("AllTasks", "ProjectTasks");
                }
            }
            return RedirectToAction("Login", "Account");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}