using System;
using System.Collections.Generic;
using System.Text;

namespace VMail.Models
{
    public class Message
    {
        public string MailBox { get; set; }
        public string Uid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public string MimeVersion { get; set; }
    }
}
