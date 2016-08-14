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

        private enum SmsType { Answer, CreateTeam, Admin, Checkin };

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
            //Do not save the SMS'es
            //foreach (var item in mm.Numbers)
            //{
            //    context.SentSmses.Add(new SentSms
            //    {
            //        Body = mm.Body,
            //        Reciever = item,
            //        Sent = DateTime.Now
            //    });
            //}
            //context.SaveChanges();
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
            var textToShow = (pa.CorrectAnswer ? p.CorrectAnswerText : p.WrongAnswerText);

            if (r.ShowNextPost && pa.NextPost != null)
            {
                textToShow += ".  Næste post er " + pa.NextPost.Title;
                if (pa.NextPost.lattitude != 0 || pa.NextPost.longitude != 0)
                {
                    textToShow += "Den er placereret på: ";
                    textToShow += "("+pa.NextPost.lattitude.ToString() + ", ";
                    textToShow += pa.NextPost.longitude.ToString()+")";
                }
            }

            return textToShow;
        }

        private string ProcessAdmin(string[] data)
        {
            return "Data length: " + data.Length;
        }

        private string CreateTeam(string sender, List<string> data)
        {
            var teamid = data[1];
            var teamname = data[1];
            if (!string.IsNullOrEmpty(data[2]))
                teamname = data[2];
            var race = context.Races.Find(int.Parse(data[0]));
            if (race == null)
                return "LØB IKKE FUNDET";
            var team = context.Teams.Where(t => t.HoldID.Equals(teamid) && t.Race.Id == race.Id).FirstOrDefault();
            if (team == null)
            {
                team = new Team
                {
                    Race = race,
                    TeamName = teamname,
                    HoldID = data[1]
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

        public string CheckInTeam(string sender, int postId, string raceId)
        {
            //Find team
            var team = FindByNumber(sender, raceId);
            if (team == null)
            {
                return "Du er ikke kendt på holdet";
            }
            var post = context.Posts.Find(postId);
            //Create checkin for team
            var checkin = new Checkin
            {
                CheckIn = DateTime.Now,
                Post = post,
                Team = team
            };
            context.Checkins.Add(checkin);
            context.SaveChanges();
            return "Holdet er checket ind på posten";
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
                    if (data.Length > 4)
                        res = AnswerPost(sender, data[2], data[3], data[4], sms);
                    else
                        res = "Fejl ved indsendelse af svar. Prøv igen";
                    break;
                case "ct":
                    type = SmsType.CreateTeam;
                    res = CreateTeam(sender, data.Skip(2).ToList());
                    break;
                case "ci":
                    type = SmsType.Checkin;
                    int postid;
                    var parse_res = int.TryParse(data[2], out postid);
                    if (!parse_res)
                    {
                        res = "Der skete en fejl";
                    }
                    else
                    {
                        res = CheckInTeam(sender, postId: postid, raceId: data[3]);
                    }
                    break;
            }
            return res;
        }
    }
}
