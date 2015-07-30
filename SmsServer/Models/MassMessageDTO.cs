using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsServer.Models
{
    public class MassMessageDTO
    {
        public IEnumerable<string> Numbers { get; set; }
        public string Body { get; set; }
    }
}