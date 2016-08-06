using SmsServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SmsServer.Controllers
{
    public class TeamsController : Controller
    {
        private SmsServerContext context = new SmsServerContext();

        public ActionResult ListTeamsForRace(int id)
        {
            var teams = context.Teams.Where(t => t.Race.Id == id).ToList();
            return View(teams);
        }

        public ActionResult AdminCreate(int id)
        {
            Session["RaceId"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult AdminCreate(string TeamName)
        {
            if (string.IsNullOrEmpty(TeamName) || string.IsNullOrWhiteSpace(TeamName))
                return View("AdminCreate");

            var team = new Team();
            team.TeamName = TeamName;
            team.HoldID = TeamName;
            var raceFromSession = (int)Session["RaceId"];
            var race = context.Races.Where(r => r.Id == raceFromSession).FirstOrDefault();
            if (race == null)
                return RedirectToAction("Index", "Home");

            team.Race = race;
            context.Teams.Add(team);
            context.SaveChanges();
            Session["TeamCreated"] = "Team was created";
            return RedirectToAction("AdminCreate", "Teams");
        }

        // GET: Teams
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string TeamName)
        {
            if (string.IsNullOrEmpty(TeamName) || string.IsNullOrWhiteSpace(TeamName))
                return View("Create");

            var team = new Team();
            team.TeamName = TeamName;
            team.HoldID = TeamName;
            var raceFromSession = (int)Session["RaceId"];
            var race = context.Races.Where(r => r.Id == raceFromSession).FirstOrDefault();
            if (race == null)
                return RedirectToAction("Index", "Home");

            team.Race = race;
            context.Teams.Add(team);
            context.SaveChanges();
            Session["TeamId"] = team.Id;
            return RedirectToAction("ShowPostForAnswer", "Home", new { postid=Session["PostToAnswer"] });
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = context.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = context.Teams.Include("Members").Where(t => t.Id == id).FirstOrDefault();
            var answers = context.Answers.Where(a => a.Team.Id == id).ToList();
            context.Answers.RemoveRange(answers);
            context.Teams.Remove(team);
            context.SaveChanges();
            if (Session["RaceId"] == null)
            {
                return RedirectToAction("Index", "Races");
            }
            else
                return RedirectToAction("Index", "Races", new { id = (int)Session["RaceId"] });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}