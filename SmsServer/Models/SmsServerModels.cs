using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Placement { get; set; }

        public string CorrectAnswerText { get; set; }
        public string WrongAnswerText { get; set; }

        public virtual PostAnswer CorrectAnswer { get; set; }
        public virtual IEnumerable<PostAnswer> Answers { get; set; }
    }

    public class PostAnswer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class Race
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Contact { get; set; }
        public string ContactNumber { get; set; }

        //Guid from application user.
        public string Owner { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
    }

    public class PredefinedSms
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public virtual Race Race { get; set; }
    }

    public class Sms
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public DateTime Received { get; set; }
    }

    public class TeamMember
    {
        public int Id { get; set; }
        public string Number { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }

        public virtual Race Race { get; set; }
        public virtual IEnumerable<TeamMember> Members { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public DateTime AnsweredAt { get; set; }

        public virtual Team Team { get; set; }
        public virtual PostAnswer ChosenAnswer { get; set; }
        public virtual Sms Sms { get; set; }
        public virtual Post Post { get; set; }
    }


}