using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsServer.Models
{
    public class AnswerStatForRace
    {
        public Team Team { get; set; }
        public bool CorrectAnswerChosen { get; set; }
        public int count { get; set; }
    }

    public class AnswerStatForPost
    {
        public Team Team { get; set; }
        public int CountOfAnswers { get; set; }
        public int CountOfIncorrectAnswers { get; set; }
    }
}
