using System;

namespace EmailProtocols.Smtp.Realization
{
    internal interface ISmtp
    {
        void ConnectToServer(string address, int port, bool isSsl = false);
        string Helo();
        string Ehlo();
        bool StartTls();
        void AuthLogin(string username, string password);
        void MailFrom(string mailFrom);
        void RcptTo(string mailTo);
        void Data(string data);
        void Rset();
        string Vrfy(string user);
        string Help(string arg = "");
        string Noop();
        void Quit();
    }
}
