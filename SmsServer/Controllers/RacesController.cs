using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmsServer.Models;

namespace SmsServer.Controllers
{
    [Authorize]
    public class RacesController : Controller
    {
        private SmsServerContext db = new SmsServerContext();

        private string GetUserNameFromRequest()
        {
            return User.Identity.Name.ToString();
        }

        private List<Race> GetRaceForUser(int? id)
        {
            var user = GetUserNameFromRequest();
            if (id == null)
            {
                return db.Races.Where(r => r.Owner == user).ToList();
            }
            else
            {
                var race = db.Races.Find(id);
                if (race == null || race.Owner != user)
                {
                    return null;
                }
                return new List<Race> { race };
            }
        }

        // GET: Races
        public ActionResult Index()
        {
            var username = GetUserNameFromRequest();
            ViewBag.TotalAnswers = (from a in db.Answers
                                   where a.Post.Race.Owner == username
                                   group a by new { a.Post.RaceID, a.Post.Id } into g
                                   select new { g.Key, count = g.Count() }).ToDictionary(g => g.Key.RaceID, g => g.count);
            var user = GetUserNameFromRequest();
            return View(GetRaceForUser(null));
        }

        // GET: Races/Details/5
        public ActionResult Details(int? id)
        {
            var user = GetUserNameFromRequest();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswersForRace = (from a in db.Answers.Include("Team").Include("Post")
                                      where a.Post.Race.Id == race.Id
                                      group a by new { a.Team, a.CorrectAnswerChosen } into g
                                      select new AnswerStatForRace { Team = g.Key.Team, CorrectAnswerChosen = g.Key.CorrectAnswerChosen, count = g.Count() }).ToList();//.OrderBy(x => x.Team);
            return View(race);
        }

        public ActionResult ShowPosts(int? id, bool printPosts = false)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            ViewBag.Teams = db.Teams.Where(t => t.Race.Id == race.Id).ToList();
            ViewBag.PrintPosts = printPosts;
            return View(race);
        }

        public ActionResult CreatePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            Session["RaceID"] = id;
            return RedirectToAction("Create", "Posts");
        }

        public ActionResult ListPosts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            Session["RaceID"] = id;
            return RedirectToAction("Index", "Posts");
        }

        // GET: Races/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Races/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] //Remembefr to put these in later: 
        public ActionResult Create([Bind(Include = "Id,Name,Contact,Start,End,ContactNumber,GatewayNumber")] Race race)
        {
            //race.Owner = User.Identity.Name.ToString();
            //race.Start = DateTime.Now;
            //race.End = DateTime.Now.AddDays(1);
            race.Owner = GetUserNameFromRequest();
            race.GatewayCode = Guid.NewGuid().ToString().Substring(0, 10);//.Replace('-','X');
            if (ModelState.IsValid)
            {
                db.Races.Add(race);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = race.Id });
            }

            return View(race);
        }

        // GET: Races/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            return View(race);
        }

        // POST: Races/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Start,End,Contact,ContactNumber")] Race race)
        {
            if (ModelState.IsValid)
            {
                Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
                if (raceFromDb == null)
                {
                    return HttpNotFound();
                }
                db.Entry(race).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(race);
        }

        // GET: Races/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Race race = GetRaceForUser(id).FirstOrDefault();
            if (race == null)
            {
                return HttpNotFound();
            }
            return View(race);
        }

        // POST: Races/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Race race = db.Races.Find(id);
            Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
            if (raceFromDb != race)
            {
                return HttpNotFound();
            }
            db.Races.Remove(race);
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
    }
}
