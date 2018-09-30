using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CopaMundivox.Models;
using System.Data.Entity.Validation;
using System.Web.Security;

namespace CopaMundivox.Controllers
{
    public class UserController : Controller
    {
        private CopaEntities db = new CopaEntities();

        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "id,Email,Password,Name,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                db.User.Add(user);
                try { 
                db.SaveChanges();
                }
                catch(DbEntityValidationException e)
                {
                    string strError = "";
                    foreach (var eve in e.EntityValidationErrors)
                        foreach (var ve in eve.ValidationErrors)
                            strError += string.Format("- Erro: \"{1}\"\n", ve.PropertyName, ve.ErrorMessage);
                    ViewBag.ValidationError = strError;
                    return View(user);
                }

                FormsAuthentication.SetAuthCookie(user.Name, false);
                Session["Name"] = user.Name;
                Session["UserId"] = user.id;

                return RedirectToAction("Index","cup");
            }

            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.ConfirmPassword = user.Password;
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Email,Password,Name,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            IEnumerable<Cup> cup = db.Cup.Where(c => c.UserId == id);
            List<Team> team = new List<Team>();
            List<CupResult> cupresult = new List<CupResult>();
            foreach(Cup item in cup)
            {
                cupresult.AddRange(db.CupResult.Where(r => r.CupId.Equals(item.Id)));
                team.AddRange(db.Team.Where(t => t.CupId.Equals(item.Id)));
            }

            db.CupResult.RemoveRange(cupresult);
            db.Team.RemoveRange(team);
            db.Cup.RemoveRange(cup);
            db.User.Remove(user);
            db.SaveChanges();
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(string Email, string Password)
        {
            User user = db.User.Where(u => u.Email.Equals(Email) && u.Password.Equals(Password)).FirstOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Name, false);
                Session["Name"] = user.Name;
                Session["UserId"] =  user.id;
                return RedirectToAction("Index", "cup");
            }
            else
            {
                ViewBag.ValidationError = "E-mail ou senha inválido.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
