using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public class SmsDTO
    {
        public string Sender { get; set; }
        public string Body { get; set; }
    }
}