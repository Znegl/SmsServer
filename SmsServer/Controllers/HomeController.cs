using SmsServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsServer.Controllers
{
    public class HomeController : Controller
    {
        private SmsServerContext context = new SmsServerContext();

        public ActionResult Index()
        {
            return View();
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

        public ActionResult AnswerPost(int postid)
        {
            var post = context.Posts.Find(postid);
            if (post == null)
            {
                //404 not found
                return HttpNotFound();
            }
            var race = post.Race;
            Session["RaceId"] = race.Id;
            if (Session["TeamId"] != null)
            {
                return RedirectToAction("ShowPostForAnswer", "Home", new { postid = postid });
                //return RedirectToAction("ConfirmTeam", new { postid = postid });
            }
            else
            {
                return RedirectToAction("ChooseTeam", new { postid = postid });
            }
        }

        public ActionResult ConfirmTeam(int postid)
        {
            var team = context.Teams.Find(Session["TeamId"]);
            if (team == null)
            {
                return RedirectToAction("ChooseTeam");
            }
            ViewBag.PostId = postid;
            return View("ConfirmTeam", team);
        }

        public ActionResult ChooseTeam(int postid)
        {
            var post = context.Posts.Find(postid);
            if (post == null)
            {
                //404 not found
                return HttpNotFound();
            }
            var race = post.Race;
            Session["RaceId"] = race.Id;
            Session["PostToAnswer"] = postid;
            var teams = context.Teams.Where(t => t.Race.Id == race.Id).ToList();
            ViewBag.PostId = postid;
            return View("ChooseTeam", teams);
        }

        public ActionResult ChooseOtherTeam(int postid)
        {
            if (Session["TeamId"] != null)
                Session.Remove("TeamId");
            return RedirectToAction("ChooseTeam", new { postid = postid });
        }

        public ActionResult ChosenTeam(int teamid, int postid)
        {
            Session["TeamId"] = teamid;
            return RedirectToAction("AnswerPost", "Home", new { postid = postid });
        }

        public ActionResult ShowPostForAnswer(int postid)
        {
            return RedirectToAction("ShowWebPost", "Posts", new { postid = postid });
        }

        public ActionResult CreateTeam(int postid)
        {
            return RedirectToAction("Create", "Teams", new { postid = postid });
        }
    }
}