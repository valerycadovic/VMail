using EmailProtocols.Smtp.Realization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailProtocols.Smtp
{
    public class GmailSmtpClient// : ISmtpClient
    {
        private readonly Realization.Smtp smtp;
        private string username;

        public GmailSmtpClient()
        {
            smtp = new Realization.Smtp();
            this.smtp.ConnectToServer("smtp.gmail.com", 465, true);
            this.smtp.Ehlo();
        }


        public bool AuthenticateAsync(string username, string password)
        {
            try
            {
                smtp.AuthLogin(username, password);
                this.username = username;
                return true;
            }
            catch(SmtpException)
            {
                return false;
            }
        }

        public void LogOut()
        {
            smtp.Quit();
        }

        public bool SendAsync(string[] receivers, string message, List<string> isSent)
        {
            isSent.Clear();
            try
            {
                smtp.MailFrom($"{username}@gmail.com");
                foreach(var reciever in receivers)
                {
                    smtp.RcptTo(reciever.Trim());
                    isSent.Add(reciever);
                }
                smtp.Data(message);
                return true;
            }
            catch(SmtpException)
            {
                return false;
            }
        }
    }
}
