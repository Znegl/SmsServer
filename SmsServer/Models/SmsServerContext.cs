using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public class SmsServerContext : DbContext
    {
        public DbSet<Sms> Smses { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAnswer> PostAnswers { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<PredefinedSms> PredefinedSmses { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Answer> Answers { get; set; }
    }
}