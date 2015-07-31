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
    public class PostAnswersController : Controller
    {
        private SmsServerContext db = new SmsServerContext();

        // GET: PostAnswers
        public ActionResult Index()
        {
            return View(db.PostAnswers.ToList());
        }

        // GET: PostAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer == null)
            {
                return HttpNotFound();
            }
            return View(postAnswer);
        }

        // GET: PostAnswers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PostAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text")] PostAnswer postAnswer)
        {
            if (ModelState.IsValid)
            {
                db.PostAnswers.Add(postAnswer);
                db.SaveChanges();
                var p = db.Posts.Find(Session["PostID"]);
                p.Answers.Add(postAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(postAnswer);
        }

        // GET: PostAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer == null)
            {
                return HttpNotFound();
            }
            return View(postAnswer);
        }

        // POST: PostAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text")] PostAnswer postAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postAnswer);
        }

        // GET: PostAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer == null)
            {
                return HttpNotFound();
            }
            return View(postAnswer);
        }

        // POST: PostAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            db.PostAnswers.Remove(postAnswer);
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
