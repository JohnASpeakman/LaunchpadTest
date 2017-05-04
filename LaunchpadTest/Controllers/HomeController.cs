using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaunchpadTest.Models;

namespace LaunchpadTest.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "consult readme.txt.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = ".";

            return View();
        }

        //view list of all users and admin status
        public ActionResult UserIndex() {
            if (Session["Admin"] != null) {
                if ((bool)Session["Admin"]) {
                    using (DBContext db = new DBContext()) {
                        return View("../Account/UserIndex.cshtml", db.userAccount.ToList());
                    }
                }
            }
            return View("../Account/UserIndex.cshtml");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}