﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmsServer.Models;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using SmsServer.Helpers;

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
                                   group a by new { a.Post.RaceID } into g
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
            var AnswersForRace = (from a in db.Answers.Include("Team").Include("Post")
                                      where a.Post.Race.Id == race.Id
                                      group a by new { a.Team, a.CorrectAnswerChosen } into g
                                      select new AnswerStatForRace { Team = g.Key.Team, CorrectAnswerChosen = g.Key.CorrectAnswerChosen, count = g.Count() }).ToList();//.OrderBy(x => x.Team);
            var teamscore = new Dictionary<Team, float>();
            foreach (var item in AnswersForRace)
            {
                if (item.Team != null)
                {
                    if (!teamscore.Keys.Contains(item.Team))
                        teamscore[item.Team] = 1.0f;
                    if (item.CorrectAnswerChosen)
                        teamscore[item.Team] *= item.count;
                    else
                        teamscore[item.Team] *= 1.0f / item.count;
                }
            }
            ViewBag.TeamScores = teamscore;
            ViewBag.AnswersForRace = AnswersForRace;
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
        public ActionResult Create([Bind(Include = "Id,Name,Contact,Start,End,ContactNumber,GatewayNumber,ShowNextPost,ShowWebAnswerQR,ShowCheckinForPost,NoOfTriesPerPost")] Race race, HttpPostedFileBase image, HttpPostedFileBase image2)
        {
            //race.Owner = User.Identity.Name.ToString();
            //race.Start = DateTime.Now;
            //race.End = DateTime.Now.AddDays(1);
            race.NoOfTriesPerPost = Math.Max(0, race.NoOfTriesPerPost);
            race.Owner = GetUserNameFromRequest();
            race.GatewayCode = Guid.NewGuid().ToString().Substring(0, 10);//.Replace('-','X');
            if (ModelState.IsValid)
            {
                db.Races.Add(race);
                db.SaveChanges();
                if (image != null)
                {
                    race.ImageMimeType = image.ContentType;
                    race.IsImageOnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"race_{race.Id}");
                    var imageData = ImageHandler.ReadAndResizeImage(image, 100, 100);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
                if (image2 != null)
                {
                    race.Image2MimeType = image2.ContentType;
                    race.IsImage2OnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"race_2_{race.Id}");
                    var imageData = ImageHandler.ReadAndResizeImage(image2, 100, 100);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
                db.SaveChanges();

                return RedirectToAction("Details", new { id = race.Id });
            }

            return View(race);
        }

        public ActionResult RemoveImage(int id, int logo=1)
        {
            Race race = db.Races.Find(id);
            if (race.Owner == GetUserNameFromRequest())
            {
                if (race != null && race.IsImageOnDisk && logo == 1)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/images/"), $"race_{race.Id}"));
                    race.ImageMimeType = "";
                    race.IsImageOnDisk = false;
                    db.SaveChanges();
                }
                else if (race != null && race.IsImage2OnDisk && logo == 2)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/images/"), $"race_{logo}_{race.Id}"));
                    race.Image2MimeType = "";
                    race.IsImage2OnDisk = false;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", new { id = id });
        }


        [AllowAnonymous]
        public FileContentResult GetImage(int id, int logo=1)
        {
            Race race = db.Races.Find(id);
            if (race != null && race.IsImageOnDisk && logo == 1)
            {
                var data = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/images/"), $"race_{race.Id}"));
                return File(data, race.ImageMimeType);
            }
            else if (race != null && race.IsImage2OnDisk && logo == 2)
            {
                var data = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath("~/images/"), $"race_{logo}_{race.Id}"));
                return File(data, race.Image2MimeType);
            }
            //else if (race != null)
            //{
            //    return File(race.Image, race.ImageMimeType);
            //}
            else
            {
                return null;
            }
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
        public ActionResult Edit([Bind(Include = "Id,Name,Start,End,Contact,ContactNumber,GatewayNumber,ShowNextPost,ShowWebAnswerQR,ShowCheckinForPost,NoOfTriesPerPost")] Race race, HttpPostedFileBase image, HttpPostedFileBase image2)
        {
            if (ModelState.IsValid)
            {
                Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
                if (raceFromDb == null)
                {
                    return HttpNotFound();
                }
                race.Owner = GetUserNameFromRequest();
                raceFromDb.Name = race.Name;
                raceFromDb.Start = race.Start;
                raceFromDb.End = race.End;
                raceFromDb.Contact = race.Contact;
                raceFromDb.ContactNumber = race.ContactNumber;
                raceFromDb.GatewayNumber = race.GatewayNumber;
                raceFromDb.ShowNextPost = race.ShowNextPost;
                raceFromDb.ShowWebAnswerQR = race.ShowWebAnswerQR;
                raceFromDb.ShowCheckinForPost = race.ShowCheckinForPost;
                raceFromDb.NoOfTriesPerPost = race.NoOfTriesPerPost;
                if (image != null)
                {
                    raceFromDb.ImageMimeType = image.ContentType;
                    raceFromDb.IsImageOnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"race_{raceFromDb.Id}");
                    var imageData = ImageHandler.ReadAndResizeImage(image, 100, 100);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
                if (image2 != null)
                {
                    raceFromDb.Image2MimeType = image2.ContentType;
                    raceFromDb.IsImage2OnDisk = true;
                    var path = Path.Combine(Server.MapPath("~/images/"), $"race_2_{raceFromDb.Id}");
                    var imageData = ImageHandler.ReadAndResizeImage(image2, 100, 100);
                    System.IO.File.WriteAllBytes(path, imageData);
                }
                //db.Entry(race).State = EntityState.Modified;
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
            var posts = db.Posts.Include(p => p.Answers).Where(p => p.RaceID == id).ToList();
            db.Posts.RemoveRange(posts);
            db.Races.Remove(race);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetPointResultForRace(int id)
        {
            Race race = db.Races.Find(id);
            Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
            if (raceFromDb != race)
            {
                return HttpNotFound();
            }
            //TODO Create the sum of points in the linq statement
            var AnswersForRace = (from a in db.Answers.Include("Team").Include("Post")
                                  where a.Post.Race.Id == race.Id
                                  select a).ToList();
                                  //group a by new { a.Team, a.CorrectAnswerChosen } into g
                                  //select new AnswerStatForRace { Team = g.Key.Team, CorrectAnswerChosen = g.Key.CorrectAnswerChosen, count = g.Count() }).ToList();//.OrderBy(x => x.Team);
            var teamscore = new Dictionary<Team, double>();
            foreach (var item in AnswersForRace)
            {
                if (item.Team != null)
                {
                    if (!teamscore.Keys.Contains(item.Team))
                        teamscore[item.Team] = item.ChosenAnswer.PointValue;
                    else
                        teamscore[item.Team] += item.ChosenAnswer.PointValue;
                }
            }
            ViewBag.TeamScores = teamscore;
            ViewBag.AnswersForRace = AnswersForRace;
            ViewBag.RaceID = id;
            return View();
        }

        public FileResult DownloadPointsAsCSV(int id)
        {
            //Race race = db.Races.Find(id);
            //Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
            //if (raceFromDb != race)
            //{
            //    return HttpNotFound();
            //}
            //TODO Create the sum of points in the linq statement
            var AnswersForRace = (from a in db.Answers.Include("Team").Include("Post")
                                  where a.Post.Race.Id == id
                                  select a).ToList();
            //group a by new { a.Team, a.CorrectAnswerChosen } into g
            //select new AnswerStatForRace { Team = g.Key.Team, CorrectAnswerChosen = g.Key.CorrectAnswerChosen, count = g.Count() }).ToList();//.OrderBy(x => x.Team);
            var teamscore = new Dictionary<Team, double>();
            foreach (var item in AnswersForRace)
            {
                if (item.Team != null)
                {
                    if (!teamscore.Keys.Contains(item.Team))
                        teamscore[item.Team] = item.ChosenAnswer.PointValue;
                    else
                        teamscore[item.Team] += item.ChosenAnswer.PointValue;
                }
            }

            var writer = new StringWriter();

            writer.Write("'");
            writer.Write("Holdnavn");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Samlet point");
            writer.WriteLine("'");

            foreach (var score in teamscore)
            {
                writer.Write("'");
                writer.Write(score.Key.TeamName);
                writer.Write("',");

                writer.Write("'");
                writer.Write(score.Value);
                writer.WriteLine("'");
            }
            writer.Flush();

            var encoder = new UnicodeEncoding();
            var data = encoder.GetBytes(writer.ToString());

            return File(data, "text/csv", "score_board.csv");
        }

        public FileResult GetAllAnswersForRace(int id)
        {
            //Race race = db.Races.Find(id);
            //Race raceFromDb = GetRaceForUser(race.Id).FirstOrDefault();
            //if (raceFromDb != race)
            //{
            //    return HttpNotFound();
            //}
            //TODO Must provide filename like stats_for_race_<id>.csv
            //TODO Should the file include single quotes?

            var answers = db.Answers.Where(a => a.Post.Race.Id == id).ToList();

            var writer = new StringWriter();

            writer.Write("'");
            writer.Write("Answered at");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Valgt svar id");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Valgt svar");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Korrekt svar");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Post id");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Post title");
            writer.Write("',");

            writer.Write("'");
            writer.Write("Holdnavn");
            writer.Write("',");

            writer.WriteLine("'");

            foreach (var answer in answers) 
            {
                writer.Write("'");
                writer.Write(answer.AnsweredAt);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.ChosenAnswer.Id);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.ChosenAnswer.Title);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.CorrectAnswerChosen);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.Post.Id);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.Post.Title);
                writer.Write("',");

                writer.Write("'");
                writer.Write(answer.Team.TeamName);
                writer.Write("',");

                writer.WriteLine("'");
            }
            writer.Flush();

            var encoder = new UnicodeEncoding();
            var data = encoder.GetBytes(writer.ToString());

            //Response.Charset = encoder.WebName;
            //Response.HeaderEncoding = encoder;
            //Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", (Request.Browser.Browser == "IE") ? HttpUtility.UrlEncode(fileName, encoding) : fileName));

            return File(data, "text/csv", "answers_for_race.csv");
        }

        public ActionResult GetTeamStatus()
        {
            var checkins = db.Checkins.Where(c => c.CheckOut == null).Include("Post").Include("Team").ToList();
            ViewBag.WithCheckouts = db.Checkins.Where(c => c.CheckOut != null).Include("Post").Include("Team").ToList();
            return View(checkins);
        }

        [AllowAnonymous]
        public ActionResult CleanNumbers()
        {
            var dateToCheck = DateTime.Now;
            var racesToFix = db.Races.Where(r => r.End < dateToCheck && r.GatewayNumber != "+4512312312").ToList();
            var smsToFix = db.Smses.Where(s => s.Received < dateToCheck && s.Sender != "+4512312312").ToList();
            var teamMembersToFix = new List<Team>();
            racesToFix.ForEach(r => 
            {
                teamMembersToFix.AddRange(db.Teams.Where(t => t.Race.Id == r.Id).Include("Members").ToList());
                r.GatewayNumber = "+4512312312";
                r.ContactNumber = "+4512312312";
            });
            smsToFix.ForEach(s =>
            {
                s.Sender = "+4512312312";
            });
            teamMembersToFix.ForEach(tm =>
            {
                tm.Members.ForEach(m =>
                {
                    m.Number = "+4512312312";
                });
            });

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
