using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CopaMundivox.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace CopaMundivox.Controllers
{
    public class CupController : Controller
    {
        private CopaEntities db = new CopaEntities();

        // GET: Cup
        public ActionResult Index()
        {
            int id = (int)Session["UserId"];
            var cup = db.Cup.Where(c => c.UserId == id).Include(c => c.Team);
            return View(cup.ToList());
        }

        // GET: Cup/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cup cup = db.Cup.Find(id);
            if (cup == null)
            {
                return HttpNotFound();
            }
            return View(cup);
        }

        // GET: Cup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,UserId")] Cup cup)
        {
            cup.UserId = (int)Session["UserId"];
            if (ModelState.IsValid)
            {
                db.Cup.Add(cup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cup);
        }

        // GET: Cup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cup cup = db.Cup.Find(id);
            if (cup == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.User, "id", "Email", cup.UserId);
            return View(cup);
        }

        // POST: Cup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,UserId")] Cup cup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.User, "id", "Email", cup.UserId);
            return View(cup);
        }

        // GET: Cup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cup cup = db.Cup.Find(id);
            if (cup == null)
            {
                return HttpNotFound();
            }
            return View(cup);
        }

        // POST: Cup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IEnumerable<CupResult> cr = db.CupResult.Where(c => c.CupId == id);
            db.CupResult.RemoveRange(cr);
            IEnumerable<Team> team = db.Team.Where(t => t.CupId == id);
            db.Team.RemoveRange(team);
            Cup cup = db.Cup.Find(id);
            db.Cup.Remove(cup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult cupResults(string teams, int cupId)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            List<TeamGroup> groups = (List<TeamGroup>)JsonConvert.DeserializeObject(teams, typeof(List<TeamGroup>));
            List<CupResult> result = new List<CupResult>();
            List<Team> teamList = new List<Team>();

            #region grava os resultados dos times na fase atual
            foreach (TeamGroup tg in groups)
            {
                foreach (CupResult cupResult in db.CupResult.Where(c => (c.TeamAId == tg.teamId ||
                               (c.TeambId == tg.teamId))))
                {
                    Team team;
                    if (cupResult.TeamAId == tg.teamId)
                    {
                         team = db.Team.Find(cupResult.TeambId);
                        team.winner = false;
                        
                    }
                    else
                    {
                        team = db.Team.Find(cupResult.TeamAId);
                        team.winner = false;
                    }
                    teamList.Add(team);
                }
            }
            #endregion

            #region insere os times que estarão na fase atual
            foreach (TeamGroup teamGroup in groups)
            {
                if (result.Where(r => (!string.IsNullOrEmpty(r.TeamAGroup) && r.TeamAGroup.Substring(0, 3).Equals(teamGroup.group.Substring(0, 3))
                    || (!string.IsNullOrEmpty(r.TeamBGroup) && r.TeamBGroup.Substring(0, 3).Equals(teamGroup.group.Substring(0, 3))))).Count() == 0)
                {
                    CupResult res = new CupResult();
                    if (teamGroup.group.EndsWith("A"))
                    {
                        res.TeamAId = teamGroup.teamId;
                        res.TeamAGroup = teamGroup.group;
                        res.CupId = cupId;
                    }
                    else if (teamGroup.group.Equals("Winne"))
                    {
                        res.TeamAId = res.TeambId = teamGroup.teamId;
                        res.TeamAGroup = res.TeamBGroup = teamGroup.group;
                        res.CupId = cupId;
                    }
                    else
                    {
                        res.TeambId = teamGroup.teamId;
                        res.TeamBGroup = teamGroup.group;
                    }
                    result.Add(res);
                    Team team = db.Team.First(t => t.Id == teamGroup.teamId);
                    team.Group = teamGroup.group;
                    if (teamGroup.group.Equals("Winne"))
                    {
                        team.winner = true;
                    }
                    else
                    {
                        team.winner = null;
                    }
                }
                else
                {
                    CupResult res = result.Where(r => r.TeamAGroup.Substring(0, 3).Equals(teamGroup.group.Substring(0, 3))).First();
                    if (teamGroup.group.EndsWith("A"))
                    {
                        res.TeamAId = teamGroup.teamId;
                        res.TeamAGroup = teamGroup.group;
                    }
                    else
                    {
                        res.TeambId = teamGroup.teamId;
                        res.TeamBGroup = teamGroup.group;
                    }
                    Team team = db.Team.First(t => t.Id == (teamGroup.teamId));
                    team.Group = teamGroup.group;
                }
            }
            #endregion

            try
            {
                db.CupResult.AddRange(result);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
            return Json("sucesso", JsonRequestBehavior.AllowGet);
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