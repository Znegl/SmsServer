using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public static class SmsExtenstions
    {
        public static SmsDTO SmsToDTO(this Sms sms)
        {
            var dto = new SmsDTO
            {
                Sender = sms.Sender,
                Body = sms.Body
            };
            return dto;
        }

        public static Sms SmsDTOToSms(this SmsDTO smsDto)
        {
            var newSms = new Sms();
            newSms.Sender = smsDto.Sender;
            newSms.Body = smsDto.Body;
            newSms.Received = DateTime.Now;
            return newSms;
        }
    }

    public static class MassMessageExtenstions
    {
        public static MassMessageDTO ToMassMessageDTO(this PredefinedSms preSms, string[] numbers)
        {
            var mm = new MassMessageDTO
            {
                Body = preSms.Text,
                Numbers = numbers
            };
            return mm;
        }
    }
}