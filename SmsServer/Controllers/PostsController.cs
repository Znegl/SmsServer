﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmsServer.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using SmsServer.Helpers;

namespace SmsServer.Controllers
{
    //TODO Redirect hvis raceid ikke sat
    [Authorize]
    public class PostsController : Controller
    {
        private SmsServerContext db = new SmsServerContext();
        private string GetUserNameFromRequest()
        {
            return User.Identity.Name.ToString();
        }
        // GET: Posts
        public ActionResult Index()
        {
            var r = db.Races.Find(Session["RaceID"]);
            return View(r.Posts);
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (post == null || !haveRace)
            {
                return HttpNotFound();
            }
            var answers = (from a in db.Answers.Include("Team").Include("Post")
                                      where a.Post.Id == post.Id
                                      group a by new { a.Team } into g
                                      select new AnswerStatForPost { Team = g.Key.Team, CountOfAnswers = g.Count() }).ToList();
            foreach (var item in answers)
            {
                item.CountOfIncorrectAnswers = db.Answers.Where(a => a.Post.Id == post.Id && a.Team.Id == item.Team.Id && a.CorrectAnswerChosen == false).Count();
            }
            ViewBag.AnswersForPost = answers;
            return View(post);
        }
        
        public ActionResult CreateAnswer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (post == null || !haveRace)
            {
                return HttpNotFound();
            }
            Session["PostID"] = id;
            return RedirectToAction("Create", "PostAnswers");
        }

        public ActionResult ListAnswers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (post == null || !haveRace)
            {
                return HttpNotFound();
            }
            Session["PostID"] = id;
            return RedirectToAction("Index", "PostAnswers");
        }
        
        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text,Placement,CorrectAnswerText,WrongAnswerText")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (post.Text == string.Empty)
                    post.Text = "";
                var r = db.Races.Find(Session["RaceID"]);
                if (r.Owner == GetUserNameFromRequest())
                {
                    r.Posts.Add(post);
                    db.Posts.Add(post);
                    db.SaveChanges();
                    if (image != null)
                    {
                        post.ImageMimeType = image.ContentType;
                        post.IsImageOnDisk = true;

                        var path = Path.Combine(Server.MapPath("~/images/"), $"post_{post.Id}");
                        var imageData = ImageHandler.ReadAndResizeImage(image, 200, 200);
                        System.IO.File.WriteAllBytes(path, imageData);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (post == null || !haveRace)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,Placement,CorrectAnswerText,WrongAnswerText,IsImageOnDisk,ImageMimeType")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var r = db.Races.Find(Session["RaceID"]);
                if (r.Owner == GetUserNameFromRequest())
                {
                    if (image != null)
                    {
                        post.ImageMimeType = image.ContentType;
                        post.IsImageOnDisk = true;

                        var path = Path.Combine(Server.MapPath("~/images/"), $"post_{post.Id}");
                        var imageData = ImageHandler.ReadAndResizeImage(image, 200, 200);
                        System.IO.File.WriteAllBytes(path, imageData);
                    }
                    post.RaceID = r.Id;
                    db.Entry(post).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (post == null || !haveRace)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            var haveRace = db.Races.Find(Session["RaceID"]).Posts.IndexOf(post) >= 0;
            if (haveRace)
            {
                //TODO Delete all post answers (using cascade delete)
                var answers = db.Answers.Where(a => a.Post.Id == id).ToList();
                post = db.Posts.Include(m => m.Answers).Where(p => p.Id == id).FirstOrDefault();
                db.Answers.RemoveRange(answers);
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public FileContentResult GetImage(int id)
        {
            Post post= db.Posts.Find(id);
            if (post != null && post.IsImageOnDisk)
            {
                var data = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/images/"), $"post_{post.Id}"));
                return File(data, post.ImageMimeType);
            }
            else if (post != null)
            {
                return File(post.Image, post.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public ActionResult RemoveImage(int id)
        {
            var r = db.Races.Find(Session["RaceID"]);
            if (r.Owner == GetUserNameFromRequest())
            {
                Post post = db.Posts.Find(id);
                if (post != null && post.IsImageOnDisk)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/images/"), $"post_{post.Id}"));
                    post.IsImageOnDisk = false;
                    post.ImageMimeType = "";
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", new { id = id });
        }

        [AllowAnonymous]
        public ActionResult ShowWebPost(int postid)
        {
            var post = db.Posts.Find(postid);

            if (post == null)
            {
                return HttpNotFound();
            }
            Session["PostToAnswer"] = postid;
            return View("ShowWebPost", post);
        }

        [AllowAnonymous]
        public ActionResult AnswerWebPost(int answerid)
        {
            var answer = db.PostAnswers.Find(answerid);

            if (answer == null)
                return HttpNotFound();

            var team = db.Teams.Find(Session["TeamId"]);
            Race r = null;
            if (team != null)
            {
                r = team.Race;
            }

            var p = db.Posts.Find(Session["PostToAnswer"]);
            var pa = p.Answers.Where(k => k.Id == answerid).FirstOrDefault();

            Answer answerIsThere = null;
            answerIsThere = db.Answers.Where(a => a.Team.Id == team.Id && a.ChosenAnswer.Id == pa.Id && a.Post.Id == p.Id).FirstOrDefault();

            var noOfTriesForTeam = db.Answers.Count(a => a.Team.Id == team.Id && a.Post.Id == p.Id);

            if ((r.NoOfTriesPerPost > 0 && noOfTriesForTeam >= r.NoOfTriesPerPost) || (noOfTriesForTeam >= p.Answers.Count))
            {
                ViewBag.TextToShow = new List<string> { "Posten er allerede besvaret" };
                return View("AnswerWebPost");
            }

            if (answerIsThere == null)
            {
                var a = new Answer
                {
                    AnsweredAt = DateTime.Now,
                    Team = team,
                    ChosenAnswer = pa,
                    Post = p,
                    Sms = null,
                    CorrectAnswerChosen = pa.CorrectAnswer
                };
                db.Answers.Add(a);
                db.SaveChanges();
            }
            var textToShow = new List<String>();
            textToShow.Add((pa.CorrectAnswer ? p.CorrectAnswerText : p.WrongAnswerText));

            if (r.ShowNextPost && pa.NextPost != null)
            {
                textToShow.Add("Næste post er " + pa.NextPost.Title);
                if (pa.NextPost.lattitude != 0 || pa.NextPost.longitude != 0)
                {
                    textToShow.Add("Den er placereret på:");
                    textToShow.Add(pa.NextPost.lattitude.ToString());
                    textToShow.Add(pa.NextPost.longitude.ToString());
                }
            }

            var checkinForTeam = db.Checkins.Where(c => c.TeamId == team.Id && c.PostId == p.Id).ToList();
            checkinForTeam.ForEach(s => s.CheckOut = DateTime.Now);
            db.SaveChanges();

            ViewBag.TextToShow = textToShow;
            return View("AnswerWebPost");
        }

        public ActionResult PlacePost(int raceid)
        {
            var r = db.Races.Find(raceid);
            if (r == null || r.Owner != GetUserNameFromRequest())
                return HttpNotFound();
            ViewBag.PostsSelectList = r.Posts.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title }).ToList();
            ViewBag.RaceId = raceid;
            return View();
        }

        public ActionResult CheckInTeam(int id, int teamid)
        {
            Session["TeamId"] = teamid;
            return CheckInTeam(id);
        }

        public ActionResult CheckInTeam(int id)
        {
            var post = db.Posts.Find(id);
            if (post == null)
            {
                //404 not found
                return HttpNotFound();
            }

            //Find team
            if (Session["TeamId"] == null)
            {
                var race = post.Race;
                Session["RaceId"] = race.Id;
                Session["PostToAnswer"] = id;
                var teams = db.Teams.Where(t => t.Race.Id == race.Id).ToList();
                ViewBag.PostId = id;
                return View("ChooseTeamForCheckin", teams);

            }
            var team = db.Teams.Find(int.Parse(Session["TeamId"].ToString()));
            //Create checkin for team
            var checkin = new Checkin
            {
                CheckIn = DateTime.Now,
                Post = post,
                Team = team
            };
            db.Checkins.Add(checkin);
            db.SaveChanges();
            return View();
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
