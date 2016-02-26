using SmsServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsServer.Controllers
{
    public class TeamsController : Controller
    {
        private SmsServerContext context = new SmsServerContext();

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