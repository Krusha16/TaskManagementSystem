using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Project Manager")]
    public class AdminsController : Controller
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admins
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllRoles()
        {
            var roles = MembershipHelper.GetAllRoles();
            return View(roles);
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRole(string Name)
        {
            MembershipHelper.AddRole(Name);
            return RedirectToAction("AllRoles");
        }

        [HttpGet]
        public ActionResult AddUserToRole()
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AddUserToRole(string UserId, string role)
        {
            ViewBag.UserId = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.role = new SelectList(db.Roles.ToList(), "Id", "Name");
            var newrole = db.Roles.Where(r => r.Id == role).Select(r => r.Name);
            MembershipHelper.AddUserToRole(UserId, newrole.FirstOrDefault());
            return RedirectToAction("AllRoles");
        }
    }
}