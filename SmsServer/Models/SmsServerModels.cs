using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsServer.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public string Placement { get; set; }
        public double longitude { get; set; }
        public double lattitude { get; set; }

        [DataType(DataType.MultilineText)]
        public string CorrectAnswerText { get; set; }
        [DataType(DataType.MultilineText)]
        public string WrongAnswerText { get; set; }

        public virtual List<PostAnswer> Answers { get; set; }

        public virtual Race Race { get; set; }
        public int RaceID { get; set; }

        [MaxLength]
        public byte[] Image { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }
    }

    public class PostAnswer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public virtual Post Post { get; set; }
        public int PostID { get; set; }

        public bool CorrectAnswer { get; set; }

        public double PointValue { get; set; }

        [MaxLength]
        public byte[] Image { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        [ForeignKey("NextPost")]
        public int? NextPostId { get; set; }
        public virtual Post NextPost { get; set; }
    }

    public class Race
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Contact { get; set; }
        public string ContactNumber { get; set; }
        public string GatewayNumber { get; set; }
        public string GatewayCode { get; set; }

        public bool ShowNextPost { get; set; }

        //Guid from application user.
        public string Owner { get; set; }

        public virtual List<Post> Posts { get; set; }
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
        public Team()
        {
            if (Members == null)
                Members = new List<TeamMember>();
        }
        public int Id { get; set; }
        public string HoldID { get; set; }
        public string TeamName { get; set; }

        public virtual Race Race { get; set; }
        public virtual List<TeamMember> Members { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public DateTime AnsweredAt { get; set; }

        public bool CorrectAnswerChosen { get; set; }

        public virtual Team Team { get; set; }
        public virtual PostAnswer ChosenAnswer { get; set; }
        public virtual Sms Sms { get; set; }
        public virtual Post Post { get; set; }
    }

    public class SentSms
    {
        public int Id { get; set; }
        public string Reciever { get; set; }
        public string Body { get; set; }
        public DateTime Sent { get; set; }
        public bool SentSuccessfully { get; set; }
    }

    public class DelayedSms
    {
        public int Id { get; set; }
        public string Reciever { get; set; }
        public string Body { get; set; }
        public DateTime SendOn { get; set; }
    }
}