using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailProtocols.Smtp.Realization
{
    internal class Smtp : ISmtp
    {
        #region Fundamental private variables
        private TcpClient tcpClient;
        private Stream stream;
        private NetworkStream networkStream;
        private SslStream sslStream;
        private bool isConnected = false;
        private bool isSsl = false;
        #endregion

        #region Constructors
        // default
        #endregion

        #region SMTP implementation

        public void AuthLogin(string username, string password)
        {
            Write("AUTH LOGIN");
            CheckAndExceptResponse(new string[] { "334" });

            string usernameBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(username));
            Write(usernameBase64);
            CheckAndExceptResponse(new string[] { "334" });

            string passwordBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(username));
            Write(passwordBase64);
            CheckAndExceptResponse(new string[] { "235" });
        }

        public void ConnectToServer(string address, int port, bool isSsl = false)
        {
            if (!isConnected || !tcpClient.Connected)
            {
                this.isSsl = isSsl;
                tcpClient = new TcpClient();

                try
                {
                    tcpClient.ConnectAsync(address, port);
                    networkStream = tcpClient.GetStream();

                    if (isSsl)
                    {
                        sslStream = new SslStream(networkStream);
                        sslStream.AuthenticateAsClient(address);
                        stream = sslStream;
                    }
                    else
                    {
                        stream = networkStream;
                    }

                    isConnected = true;
                    string response = Response();

                    if (!CheckResponse(response, new string[] { "220" }))
                    {
                        isConnected = false;
                        tcpClient.Close();
                        // own
                        throw new SmtpException(response);
                    }
                    else
                    {
                        isConnected = true;
                    }
                }
                catch (SocketException)
                {
                    tcpClient.Dispose();
                    isConnected = false;
                    throw;
                }

            }
        }

        public void Data(string data)
        {
            Write("DATA");
            CheckAndExceptResponse(new string[] { "354" });
            string[] lines = data.Split('\n');
            StringBuilder message = new StringBuilder();

            foreach (var line in lines)
            {
                if (line[0] == '.')
                {
                    message.Append(".");
                    message.Append(line);
                    message.Append("\n");
                }
                else
                {
                    message.Append(line);
                    message.Append("\n");
                }
            }

            Write(message.ToString() + "\r\n.\r\n");
            CheckAndExceptResponse(new string[] { "250" });
        }

        public string Ehlo()
        {
            Write("EHLO " + Environment.MachineName);
            string response = Response();

            if (!CheckResponse(response, new string[] { "250" }))
            {
                // own
                throw new SmtpException(response);
            }

            return response;
        }

        public string Helo()
        {
            Write("HELO " + Environment.MachineName);
            string response = Response();

            if (!CheckResponse(response, new string[] { "250" }))
            {
                // own
                throw new SmtpException(response);
            }

            return response;
        }

        public string Help(string arg = "")
        {
            Write($"HELP {arg}");
            string response = Response();
            if (!CheckResponse(response, new string[] { "211", "214" }))
            {
                throw new Exception(response);
            }

            return response;
        }

        public void MailFrom(string mailFrom)
        {
            Write($"MAIL FROM:<{mailFrom}>");
            CheckAndExceptResponse(new string[] { "250" });
        }

        public string Noop()
        {
            Write("NOOP");
            return Response();
        }

        public void Quit()
        {
            Write("QUIT");
            if (CheckResponse(Response(), new string[] { "211" }))
            {
                isConnected = false;
                tcpClient.Dispose();
            }
        }

        public void RcptTo(string mailTo)
        {
            Write($"RCPT TO:<{mailTo}>");
            CheckAndExceptResponse(new string[] { "250" });
        }

        public void Rset()
        {
            Write("RSET");
            CheckAndExceptResponse(new string[] { "250" });
        }

        public bool StartTls()
        {
            Write("STARTTLS");
            if (!CheckResponse(Response(), new string[] { "220" }))
                return false;
            return true;
        }

        public string Vrfy(string user)
        {
            Write("VRFY " + user);
            string response = Response();
            if (CheckResponse(response, new string[]
            {
                "250",
                "251",
                "252"
            }))
            {
                return response;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Private Section

        private string Response()
        {
            if (isConnected)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] buffer = new byte[2048];

                try
                {
                    stream.Read(buffer, 0, buffer.Length);
                    string response = encoding.GetString(buffer);
                    return encoding.GetString(buffer);
                }
                catch (SocketException)
                {
                    throw;
                }
            }
            else
            {
                // Create own exception later
                throw new SmtpException("User is not connected to SMTP server. Read error");
            }
        }

        private bool CheckResponse(string response, string[] goods)
        {
            Regex regex = new Regex(@"^\d{3}");
            Match match = regex.Match(response);

            if (match.Success)
            {
                string numberFromServer = match.Value;
                foreach (var number in goods)
                {
                    if (string.Equals(numberFromServer, number))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckAndExceptResponse(string[] goods)
        {
            string response = Response();

            if (!CheckResponse(response, goods))
            {
                // own exception
                throw new SmtpException(response);
            }
        }

        private void Write(string command)
        {
            if (isConnected)
            {
                if (!command.EndsWith("\r\n"))
                {
                    command += "\r\n";
                }
                byte[] buffer = Encoding.UTF8.GetBytes(command);
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
                catch (SocketException)
                {
                    throw;
                }
            }
            else
            {
                throw new SmtpException("User is not connected to SMTP server. Write error");
            }
        }
        #endregion
    }
}
