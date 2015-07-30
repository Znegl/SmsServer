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

        private enum SmsType { Answer, CreateTeam, Admin};

        [HttpPost]
        public MassMessageDTO HandleIncomingSms([FromBody]SmsDTO incomingSms)
        {
            var smsSender = incomingSms.Sender;
            var body = incomingSms.Body;
            var sms = SmsDTO.SmsDTOToSms();
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
            var team = FindByNumber(sender);
            var r = team.Race.Single();
            var p = r.Posts.Where(p => p.Id == postId).FirstOrDefault();
            var pa = p.Answers.Where(a => a.Id == answerid).FirstOrDefault();
            var a = new Answer {
                AnsweredAt = DateTime.Now,
                Team = team,
                PostAnswer = pa,
                Post = p,
                Sms = sms
            }
            return "You answered something";
        }

        private string ProcessAdmin(string[] data)
        {
            return "Data length: " + data.Length;
        }

        private string CreateTeam(string sender, List<string> data)
        {
            if (!string.IsNullOrEmpty(data[2]))
                return "NAME Creating team with name: " + data[2];
            return "ID Createing team with id: " + data[1];
        }

        //TODO Error handling when sms data is not in correct format and correct length
        //TODO Convert data here
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
