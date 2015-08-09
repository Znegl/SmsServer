using SmsServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmsServer.Controllers
{
    public class SmsController : ApiController
    {
        private SmsServerContext context = new SmsServerContext();

        private enum SmsType { Answer, CreateTeam, Admin };

        [HttpPost]
        [SmsGatewayAuthorizationFilter]
        public MassMessageDTO HandleIncomingSms([FromBody]SmsDTO incomingSms)
        {
            var smsSender = incomingSms.Sender;
            var body = incomingSms.Body;
            var sms = incomingSms.SmsDTOToSms();
            context.Smses.Add(sms);
            context.SaveChanges();
            var res = processSms(sms);
            var mm = new MassMessageDTO
            {
                Numbers = new string[] { smsSender },
                Body = res
            };
            return mm;
        }

        private Team FindByNumber(string sender, string raceId)
        {
            var id = int.Parse(raceId);
            return context.Teams.Where(t => t.Race.Id == id).Where(t => t.Members.Any(m => m.Number == sender)).FirstOrDefault();
        }

        private string AnswerPost(string sender, string raceId, string postId, string answerid, Sms sms)
        {
            var team = FindByNumber(sender, raceId);
            Race r = null;
            if (team != null)
                r = team.Race;
            else
                r = context.Races.Find(int.Parse(raceId));
            var p = r.Posts.Where(q => q.Id == int.Parse(postId)).FirstOrDefault();
            var pa = p.Answers.Where(k => k.Id == int.Parse(answerid)).FirstOrDefault();
            var a = new Answer
            {
                AnsweredAt = DateTime.Now,
                Team = team,
                ChosenAnswer = pa,
                Post = p,
                Sms = sms,
                CorrectAnswerChosen = pa.CorrectAnswer
            };
            context.Answers.Add(a);
            context.SaveChanges();
            return (pa.CorrectAnswer ? p.CorrectAnswerText : p.WrongAnswerText);
        }

        private string ProcessAdmin(string[] data)
        {
            return "Data length: " + data.Length;
        }

        private string CreateTeam(string sender, List<string> data)
        {
            var teamname = data[1];
            if (!string.IsNullOrEmpty(data[2]))
                teamname = data[2];
            var race = context.Races.Find(int.Parse(data[0]));
            if (race == null)
                return "LØB IKKE FUNDET";
            var team = context.Teams.Find(int.Parse(data[1]));
            if (team == null)
            {
                team = new Team
                {
                    Race = race,
                    TeamName = teamname
                };
                var tm = new TeamMember
                {
                    Number = sender
                };
                context.TeamMembers.Add(tm);
                context.SaveChanges();
                context.Teams.Add(team);
                context.SaveChanges();
                team.Members.Add(tm);
                context.SaveChanges();
            }
            else
            {
                var tm = new TeamMember
                {
                    Number = sender
                };
                context.TeamMembers.Add(tm);
                context.SaveChanges();
                team.Members.Add(tm);
                context.SaveChanges();
            }
            return "Hold oprettet";
        }

        //TODO Error handling when sms data is not in correct format and correct length
        private string processSms(Sms sms)
        {
            string sender = sms.Sender;
            string msg = sms.Body;
            var data = msg.Split('#');
            SmsType type = SmsType.Answer;
            string res = "";
            switch (data[1])
            {
                case "ac":
                    type = SmsType.Admin;
                    res = ProcessAdmin(data.Skip(2).ToArray());
                    break;
                case "ic":
                    type = SmsType.Answer;
                    res = AnswerPost(sender, data[2], data[3], data[4], sms);
                    break;
                case "ct":
                    type = SmsType.CreateTeam;
                    res = CreateTeam(sender, data.Skip(2).ToList());
                    break;
            }
            return res;
        }
    }
}
