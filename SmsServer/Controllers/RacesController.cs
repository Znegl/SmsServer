﻿using System;
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
    //TODO Make sure that edit, create and delete post also check for user
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
