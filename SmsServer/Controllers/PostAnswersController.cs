using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmsServer.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace SmsServer.Controllers
{

    public class PostAnswersController : Controller
    {
        private SmsServerContext db = new SmsServerContext();

        // GET: PostAnswers
        public ActionResult Index()
        {
            if (Session["PostID"] != null)
            {
                var postId = int.Parse(Session["PostID"].ToString());
                return View(db.PostAnswers.Where(a => a.PostID == postId).ToList());
            }
            return View(new List<PostAnswer>());
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

        public ActionResult SetCorrectAnswer(int? id)
        {
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer == null)
            {
                return HttpNotFound();
            }
            postAnswer.CorrectAnswer = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UnsetCorrectAnswer(int? id)
        {
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer == null)
            {
                return HttpNotFound();
            }
            postAnswer.CorrectAnswer = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private byte[] ReadAndResizeImage(HttpPostedFileBase image, int maxWidth, int maxHeight)
        {
            var imageData = new byte[image.ContentLength];
            var imgOrig = Image.FromStream(image.InputStream);
            if (imgOrig.Width < maxWidth && imgOrig.Height < maxHeight)
            {
                MemoryStream ms2 = new MemoryStream();
                imgOrig.Save(ms2, imgOrig.RawFormat);
                return ms2.ToArray();
            }

            var lnRatio = 0.0m;
            int lnNewWidth = 0;
            int lnNewHeight = 0;

            if (imgOrig.Width > imgOrig.Height)
            {
                lnRatio = (decimal)maxWidth / imgOrig.Width;
                lnNewWidth = maxWidth;
                decimal lnTemp = imgOrig.Height * lnRatio;
                lnNewHeight = (int)lnTemp;
            }
            else
            {
                lnRatio = (decimal)maxHeight / imgOrig.Height;
                lnNewHeight = maxHeight;
                decimal lnTemp = imgOrig.Width * lnRatio;
                lnNewWidth = (int)lnTemp;
            }

            Image resizedImg = new Bitmap(lnNewWidth, lnNewHeight, imgOrig.PixelFormat);
            Graphics g = Graphics.FromImage(resizedImg);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new Rectangle(0, 0, lnNewWidth, lnNewHeight);
            g.DrawImage(imgOrig, rect);
            MemoryStream ms = new MemoryStream();
            resizedImg.Save(ms, imgOrig.RawFormat);
            return ms.ToArray();
        }
        // GET: PostAnswers/Create
        public ActionResult Create()
        {
            var race = db.Races.Find(Session["RaceID"]);
            var postsSelectItems = new List<SelectListItem>();
            if (race != null)
            {
                var posts = race.Posts;

                postsSelectItems = posts.Select(p => new SelectListItem { Text = p.Title, Value = p.Id.ToString() }).ToList();
                postsSelectItems.Add(new SelectListItem { Text = "Ingen", Value = "0", Selected = true });
            }
            ViewBag.PostsToChoose = postsSelectItems;
            return View();
        }

        // POST: PostAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text,PointValue")] PostAnswer postAnswer, int nextPostId, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var p = db.Posts.Find(Session["PostID"]);
                var race = db.Races.Find(Session["RaceID"]);
                if (p.Race != race)
                {
                    return HttpNotFound();
                }

                if (nextPostId > 0)
                {
                    var nextPost = db.Posts.Find(nextPostId);
                    postAnswer.NextPost = nextPost;
                    postAnswer.NextPostId = nextPost.Id;
                }

                p.Answers.Add(postAnswer);
                postAnswer.Post = p;
                db.PostAnswers.Add(postAnswer);
                db.SaveChanges();
                if (image != null)
                {
                    postAnswer.ImageMimeType = image.ContentType;
                    postAnswer.IsImageOnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"postanswer_{postAnswer.Id}");
                    var imageData = ReadAndResizeImage(image, 200, 200);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
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
            var race = db.Races.Find(Session["RaceID"]);
            var postsSelectItems = new List<SelectListItem>();
            if (race != null)
            {
                var posts = race.Posts;

                postsSelectItems = posts.Select(p => new SelectListItem { Text = p.Title, Value = p.Id.ToString() }).ToList();
                postsSelectItems.Add(new SelectListItem { Text = "Ingen", Value = "0", Selected = true });
            }
            ViewBag.PostsToChoose = postsSelectItems;
            return View(postAnswer);
        }

        // POST: PostAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,PointValue")] PostAnswer postAnswer, int nextPostId, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var p = db.Posts.Find(Session["PostID"]);
                var race = db.Races.Find(Session["RaceID"]);
                if (p.Race != race)
                {
                    return HttpNotFound();
                }
                if (image != null)
                {
                    postAnswer.ImageMimeType = image.ContentType;
                    postAnswer.IsImageOnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"postanswer_{postAnswer.Id}");
                    var imageData = ReadAndResizeImage(image, 200, 200);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
                if (nextPostId > 0)
                {
                    var nextPost = db.Posts.Find(nextPostId);
                    postAnswer.NextPost = nextPost;
                }
                //TODO Why is the postAnswer not updated with the next post?
                if (nextPostId > 0)
                {
                    var nextPost = db.Posts.Find(nextPostId);
                    postAnswer.NextPost = nextPost;
                    postAnswer.NextPostId = nextPost.Id;
                }

                postAnswer.Post = p;
                postAnswer.PostID = p.Id;
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
            var p = db.Posts.Find(Session["PostID"]);
            var race = db.Races.Find(Session["RaceID"]);
            if (p.Race != race)
            {
                return HttpNotFound();
            }
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            db.PostAnswers.Remove(postAnswer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public FileContentResult GetImage(int id)
        {
            PostAnswer postAnswer = db.PostAnswers.Find(id);
            if (postAnswer != null && postAnswer.IsImageOnDisk)
            {
                var data = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/images/"), $"postanswer_{postAnswer.Id}"));
                return File(data, postAnswer.ImageMimeType);
            }
            else if (postAnswer != null)
            {
                return File(postAnswer.Image, postAnswer.ImageMimeType);
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
