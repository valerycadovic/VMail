using EmailProtocols.Imap.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailProtocols.Imap.Realization
{
    public class ImapClient : IDisposable
    {
        private ImapKernel imap;

        public ImapClient()
        {
            imap = new ImapKernel();
        }

        public async Task<bool> Login(string username, string password)
        {
            string response = await imap.ReceiveResponse($"$ LOGIN {username} {password}");
            return response.Contains("OK");
        }

        public async Task Logout()
        {
            await imap.ReceiveResponse("$ LOGOUT");
        }

        public async Task<string[]> GetFolders()
        {
            List<string> folders = new List<string>();

            string response = await imap.ReceiveResponse("$ LIST \"\" \"*\"\r\n");
            string[] division = response.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Regex regex = new Regex(@"\s([a-zA-Z-\[\]\""]+)$");
            foreach (var item in division)
            {
                if (!item.Contains("\\HasChildren")) 
                {
                    Match match = regex.Match(item);
                    if (match.Value != string.Empty)
                    {
                        folders.Add(match.Value.Replace(@"""", string.Empty).Remove(0, 1));
                    }
                }
            }

            return folders.ToArray();
        }
        

        public async Task<bool> SelectFolder(string folder)
        {
            string response = await imap.ReceiveResponse($"$ SELECT {folder}");
            return response.Contains("$ OK");
        }

        public async Task DeleteMessage(string uid)
        {
            await imap.ReceiveResponse($"$ UID STORE {uid} +FLAGS Deleted");
            await imap.ReceiveResponse("$ EXPUNGE");
        }

        public async Task<string> GetSubject(string uid)
        {
            return await GetHeaderField(uid, "Subject");
        }

        public async Task<string> GetFrom(string uid)
        {
            return await GetHeaderField(uid, "From");
        }

        public async Task<string> GetTo(string uid)
        {
            return await GetHeaderField(uid, "To");
        }

        public async Task<string> GetDate(string uid)
        {
            return await GetHeaderField(uid, "Date");
        }
        

        public async Task<string[]> GetUids()
        {
            string response = await imap.ReceiveResponse("$ UID SEARCH ALL");
            Regex regex = new Regex(@"(?<=SEARCH )((\d*) )*(\d*)");
            string uids = regex.Match(response).Value;
            return uids.Split(' ');
        }
        
        private async Task<string> GetHeaderField(string uid, string field)
        {
            string response = await imap.ReceiveResponse($"$ UID FETCH {uid} BODY[HEADER.FIELDS ({field})]");
            int index = response.IndexOf("$ OK");
            if (index == -1)
            {
                throw new Exception($"error while Get{field}");
            }
            Regex regex = new Regex($@"(?<=\r\n{field}: )([\s\S]*?)(?=\r\n\r\n\))");


            return MessageDecoder.DecodeEncodedLine(regex.Match(response).Value);
        }

        public async Task<string> GetBody(string uid)
        {
            string header = await imap.ReceiveResponse($"$ UID FETCH {uid} BODY[HEADER]");
            string body = await imap.ReceiveResponse($"$ UID FETCH {uid} BODY[TEXT]");

            Regex regexEncoding = new Regex(@"(?<=\r\nContent-Transfer-Encoding: )([\s\S]*?)(?=(\r\n))");
            Regex regexType = new Regex(@"(?<=\r\nContent-Type: )(.*?)(?=(;))");

            var typeMatch = regexType.Match(header).Value;

            string encoding = string.Empty;

            if (typeMatch == "text/html" || typeMatch == "text/plain")
            {
                var div = body.Split(new string[] { "\r\n" }, StringSplitOptions.None)
                    .Where(n => !n.StartsWith("* ") && !n.StartsWith("$ ") && !(n == ")"));

                body = string.Empty;
                foreach (var item in div)
                {
                    body += item + "\r\n";
                }

                encoding = regexEncoding.Match(header).Value;
                return DecodeBody(body, encoding, "utf-8");
            }

            int index = body.IndexOf("Content-Type: text/html");
            if (index == -1)
            {
                index = body.IndexOf("Content-Type: text/plain");
            }

            int startIndex = body.IndexOf("\r\n", index);
            int endIndex = body.IndexOf("\r\n--", startIndex);
            var sbstr = body.Substring(startIndex, endIndex - startIndex);
            encoding = regexEncoding.Match(sbstr).Value;
            sbstr = sbstr.Substring(sbstr.IndexOf("\r\n", 2));
            body = sbstr.Substring(sbstr.IndexOf("\r\n") + "\r\n".Length);
            return DecodeBody(body, encoding, "utf-8");
        }

        private static string DecodeBody(string body, string encoding, string charset)
        {
            switch (encoding)
            {
                case "quoted-printable":
                    return MessageDecoder.DecodeQP(body, "utf-8");
                case "base64":
                    try
                    {
                        var bytes = Convert.FromBase64String(body);
                        return Encoding.GetEncoding(charset).GetString(bytes);
                    }
                    catch
                    {
                        return body;
                    }
                default:
                    return body;
            }
        }

        public void Dispose()
        {
            imap.Dispose();
        }
    }
}
