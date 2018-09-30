using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CopaMundivox.Models;

namespace CopaMundivox.Controllers
{
    public class TeamController : Controller
    {
        private CopaEntities db = new CopaEntities();

        public ActionResult Create(int? CupId)
        {
            if (CupId == null)
                return RedirectToAction("Index", "Home");

            int id = int.Parse(CupId.ToString());
            Cup cup = db.Cup.First(c => c.Id == id);
            if (cup.Team == null || cup.Team.Count() <= 4)
            {
                ViewBag.CupId = CupId;
                return View();
            }
            else
                return RedirectToAction("Details", "Cup", new { id=CupId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Group,CupId,winner")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Team.Add(team);
                db.SaveChanges();
                return RedirectToAction("Details", "Cup", new { id = team.CupId });
            }

            ViewBag.CupId = new SelectList(db.Cup, "Id", "Name", team.CupId);
            return View(team);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Team.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.CupId = new SelectList(db.Cup, "Id", "Name", team.CupId);
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Group,CupId,winner")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("details", "Cup", new { id = team.CupId }); ;
            }
            ViewBag.CupId = new SelectList(db.Cup, "Id", "Name", team.CupId);
            return View(team);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Team.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            int cupResult = db.CupResult.Where(c => c.TeamAId == team.Id || c.TeambId == team.Id).Count();
            ViewBag.cupResult = cupResult;
            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            Team team = db.Team.Find(id);
            int cupId = team.CupId;
            IEnumerable<CupResult> cr = db.CupResult.Where(c => c.CupId == cupId);
            db.CupResult.RemoveRange(cr);
            db.Team.Remove(team);
            foreach(Team t in db.Team.Where(t => t.CupId == cupId))
            {
                t.Group = "";
            }
            db.SaveChanges();
            return RedirectToAction("details", "Cup", new { id = cupId });
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
