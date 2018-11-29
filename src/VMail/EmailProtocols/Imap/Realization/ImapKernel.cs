using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EmailProtocols.Imap.Realization
{
    internal class ImapKernel : IDisposable
    {
        #region private fields
        private TcpClient tcp;
        private SslStream ssl;
        #endregion
        
        public ImapKernel(string server = "imap.gmail.com", int port = 993)
        {
            try
            {
                tcp = new TcpClient(server, port);
                ssl = new SslStream(tcp.GetStream());
                ssl.AuthenticateAsClient(server);
                Task.Run(() => ReceiveResponse(""));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<string> ReceiveResponse(string query)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                if (query != "")
                {
                    if (tcp.Connected)
                    {
                        byte[] dummy = Encoding.ASCII.GetBytes(query + "\r\n");
                        await ssl.WriteAsync(dummy, 0, dummy.Length);
                    }
                    else
                    {
                        throw new ApplicationException("Tcp connection failed");
                    }

                }
            }
            catch
            {
                throw;
            }

            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead;
                    bytesRead = await ssl.ReadAsync(buffer, 0, 2048);

                    if (bytesRead == 0)
                    {
                        throw new EndOfStreamException("Error while reading");
                    }
                    string str = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
                    sb.Append(str);
                    if (SearchEndOfMessage(str)) break;
                }
            }
            catch
            {
                throw;
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            ssl.Close();
            ssl.Dispose();
            tcp.Close();
            tcp.Dispose();
        }

        #region private section
        private bool SearchEndOfMessage(string str)
        {
            return str.Contains("$ OK")
                || str.Contains("$ NO")
                || str.Contains("$ BAD")
                || str.Contains("Gimap")
                || str.Contains("* BAD");
        }

        #endregion
    }
}
