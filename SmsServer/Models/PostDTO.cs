using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Placement { get; set; }
        public double longitude { get; set; }
        public double lattitude { get; set; }

        public string CorrectAnswerText { get; set; }
        public string WrongAnswerText { get; set; }

        public int RaceID { get; set; }

        public byte[] Image { get; set; }
        public string ImageMimeType { get; set; }
    }
}