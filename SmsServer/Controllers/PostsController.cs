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
                item.CountOfIncorrectAnswers = db.Answers.Where(a => a.Post.Id == post.Id && a.Team == item.Team && a.CorrectAnswerChosen == false).Count();
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
                var r = db.Races.Find(Session["RaceID"]);
                if (r.Owner == GetUserNameFromRequest())
                {
                    if (image != null)
                    {
                        post.ImageMimeType = image.ContentType;
                        post.Image = new byte[image.ContentLength];
                        image.InputStream.Read(post.Image, 0, image.ContentLength);
                    }
                    r.Posts.Add(post);
                    db.Posts.Add(post);
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
        public ActionResult Edit([Bind(Include = "Id,Title,Text,Placement,CorrectAnswerText,WrongAnswerText")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var r = db.Races.Find(Session["RaceID"]);
                if (r.Owner == GetUserNameFromRequest())
                {
                    if (image != null)
                    {
                        post.ImageMimeType = image.ContentType;
                        post.Image = new byte[image.ContentLength];
                        image.InputStream.Read(post.Image, 0, image.ContentLength);
                    }

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
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public FileContentResult GetImage(int id)
        {
            Post post= db.Posts.Find(id);
            if (post != null)
            {
                return File(post.Image, post.ImageMimeType);
            }
            else
            {
                return null;
            }
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
