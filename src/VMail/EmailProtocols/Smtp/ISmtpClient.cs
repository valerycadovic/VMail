using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailProtocols.Smtp
{
    public interface ISmtpClient
    {
        Task<bool> AuthenticateAsync(string username, string password);
        Task<bool> SendAsync(string[] receivers, string message, List<string> isSent);
        void LogOut();
    }
}
