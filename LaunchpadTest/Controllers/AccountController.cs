using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaunchpadTest.Models;
using System.Net;

namespace LaunchpadTest.Controllers {
    public class AccountController : Controller {

        public ActionResult UserIndex() {
            if (Session["Admin"] != null) {
                if ((bool)Session["Admin"]) {
                    using (DBContext db = new DBContext()) {
                        return View(db.userAccount.ToList());
                    }
                }
            }
            return View();
        }


        public ActionResult Register() {

            return View();
        }



        //register method
        [HttpPost]
        public ActionResult Register(UserAccount account) {

            if (ModelState.IsValid) {
                using (DBContext db = new DBContext()) {
                    //check if username does not exist
                    if (db.userAccount.Where(u => u.UserName == account.UserName).FirstOrDefault() == null) {

                        db.userAccount.Add(account);
                        db.SaveChanges();
                        ModelState.Clear();
                        ViewBag.Message = account.UserName + "  " + account.UserID + "registration successfull";
                        //remain on current oage to add multiple users simultaneiousley for testing
                    }
                    else {
                        ModelState.AddModelError("", "Username not valid, please enter another username");
                    }
                }

            }
            return View();
        }

        public ActionResult Login() {

            return View();
        }

        [HttpPost]
        public ActionResult Login(UserAccount userIn) {
            using (DBContext db = new DBContext()) {
                //attempt to log in
                var user = db.userAccount.Where(u => u.UserName == userIn.UserName && u.Password == userIn.Password).FirstOrDefault();
                if (user != null) {
                    //set log in sesion
                    Session["UserID"] = user.UserID.ToString();
                    Session["UserName"] = user.UserName.ToString();
                    Session["Admin"] = user.Admin;
                    return RedirectToAction("LoggedIn");
                }
                else {
                    ModelState.AddModelError("", "Invalid log in");
                }
            }
            return View();
        }

        public ActionResult LoggedIn() {
            if (Session["UserId"] != null) {
                return View();
            }
            else {
                return RedirectToAction("Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff() {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //delete user
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            using (DBContext db = new DBContext()) {
                UserAccount personalDetail = db.userAccount.Find(id);
                db.userAccount.Remove(personalDetail);
                db.SaveChanges();
                return RedirectToAction("UserIndex");
            }
        }
    }
}