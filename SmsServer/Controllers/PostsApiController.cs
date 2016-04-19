using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SmsServer.Models;

namespace SmsServer.Controllers
{
    [Authorize]
    public class PostsApiController : ApiController
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

        // GET: api/PostsApi
        [HttpGet]
        public List<Post> GetAll(int id)
        {
            var race = GetRaceForUser(id).First();
            if (race == null)
                return new List<Post>();
            return race.Posts;
        }

        // PUT: api/PostsApi/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutPost(int id, Post post)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != post.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(post).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PostExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //[HttpPost]
        //[Route("api/updateLocationForPost/{postid}")]// /{longitude}/{lattitude}")]
        public IHttpActionResult UpdateLocationForPost(int postid, [FromBody] PostPos postPos)
        {
            var longitude = postPos.Longitude;
            var lattitude = postPos.Lattitude;
            var post = db.Posts.Find(postid);

            var race = GetRaceForUser(post.RaceID).First();

            if (race.Owner != GetUserNameFromRequest())
                return StatusCode(HttpStatusCode.NotFound);

            if (race == null)
                return StatusCode(HttpStatusCode.Unauthorized);

            post.longitude = longitude;
            post.lattitude = lattitude;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
        }

        //// POST: api/PostsApi
        //[ResponseType(typeof(Post))]
        //public IHttpActionResult PostPost(Post post)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Posts.Add(post);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = post.Id }, post);
        //}

        //// DELETE: api/PostsApi/5
        //[ResponseType(typeof(Post))]
        //public IHttpActionResult DeletePost(int id)
        //{
        //    Post post = db.Posts.Find(id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Posts.Remove(post);
        //    db.SaveChanges();

        //    return Ok(post);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostExists(int id)
        {
            return db.Posts.Count(e => e.Id == id) > 0;
        }
    }
}