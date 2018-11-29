using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Email.Imap
{
    /// <summary>
    /// Keeps IMAP connection
    /// </summary>
    public class ImapClient
    {
        private readonly int _simultConnections;
        private const int MAX_SIMULT = 5;

        private readonly StreamOperator[] _streamOperators = new StreamOperator[MAX_SIMULT];
        private readonly TcpClient[] _tcpClients = new TcpClient[MAX_SIMULT];
        private readonly Stream[] _streams = new Stream[MAX_SIMULT];
        private readonly ITlsEncryptor _tls;
        private readonly List<ImapMessageInfo> _messagesCash = new List<ImapMessageInfo>();
        private List<string> _uidsCash = new List<string>();
        private List<string> _currentUids = new List<string>();
        private readonly Semaphore _hSemaphore;
        private readonly Queue<int> _freeStreams;
        private readonly object _o = new object();
        private readonly object _o0 = new object();
        private readonly object _o1 = new object();

        /// <summary>
        /// Pagination step
        /// </summary>
        public int Step { get; set; } = 10;

        /// <summary>
        /// Count of messages on the current page
        /// </summary>
        public int MessagesCount => _uidsCash.Count;

        /// <summary>
        /// Gets the messages loaded.
        /// </summary>
        public int MessagesLoaded { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImapClient"/> class.
        /// </summary>
        /// <param name="tls">The TLS encryptor.</param>
        /// <param name="simultConnections">The simultaneous connections count.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when tls is null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when connections limit exceeded
        /// </exception>
        public ImapClient(ITlsEncryptor tls, int simultConnections = 5)
        {
            _tls = tls ?? throw new ArgumentNullException(nameof(ImapClient) + " Null argument");
            _simultConnections = simultConnections <= MAX_SIMULT ? simultConnections : throw new ArgumentOutOfRangeException();
            _hSemaphore = new Semaphore(simultConnections, simultConnections);
            _freeStreams = new Queue<int>();

            // fill free streams
            for (int i = 0; i < simultConnections; _freeStreams.Enqueue(i++))
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImapClient"/> class.
        /// </summary>
        /// <param name="simultConnections">The simult connections.</param>
        public ImapClient(int simultConnections = 5) : this(new DefaultTlsEncryptor(), simultConnections) { }

        /// <summary>
        /// Connects asynchronous.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        public async Task ConnectAsync(string login, string password)
        {
            var host = login.Substring(login.IndexOf("@") + 1);
            var tasks = new List<Task>();

            for (int i = 0; i < _simultConnections; i++)
            {
                int j = i;  // defeat loop variable closure
                
                // connect and authenticate
                tasks.Add(Task.Run(async () =>
                {
                    _tcpClients[j] = new TcpClient($"imap.{host}", 993);

                    _streams[j] = await _tls.AuthenticateAsync(_tcpClients[j].GetStream(), $"imap.{host}");

                    _streamOperators[j] = new EmailStreamOperator(_streams[j]);

                    await _streamOperators[j].ReceiveResponse("", s => s.Contains("* OK"));
                    var resp = await _streamOperators[j]
                        .ReceiveResponse(
                            $"$ LOGIN {login} {password}",
                            s => s.Contains("$ OK")
                                 || s.Contains("$ NO"));

                    // authentication failure
                    if (resp.First().Contains("$ NO"))
                    {
                        throw new InvalidOperationException("auth error");
                    }
                }));
            }

            await Task.WhenAll(tasks);
            await LoadUids();
        }

        /// <summary>
        /// Sets the flag on the message.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="sign">The sign.</param>
        /// <param name="flag">The flag.</param>
        /// <exception cref="FormatException">sign is non-valid</exception>
        public async Task SetFlag(string uid, char sign, string flag)
        {
            if ("+-".IndexOf(sign) == -1)
            {
                throw new FormatException(nameof(sign) + " must be + or -");
            }

            if (!flag.StartsWith(@"\"))
            {
                flag = @"\" + flag;
            }

            _hSemaphore.WaitOne();
            int i = GetFreeStream();
            
            // set flag
            await _streamOperators[i]
                .ReceiveResponse($@"$ UID STORE {uid} {sign}FLAGS {flag}", s => s.Contains("$ OK"));

            // apply changes
            await _streamOperators[i]
                .ReceiveResponse("$ EXPUNGE", s => s.Contains("$ OK"));

            ReleaseStream(i);
            _hSemaphore.Release();
        }

        /// <summary>
        /// Selects the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>does folder exist</returns>
        public async Task<bool> SelectFolder(string folder)
        {
            _hSemaphore.WaitOne();

            int i = GetFreeStream();
            var a = await _streamOperators[i]
                .ReceiveResponse($"$ SELECT {folder}",
                s => s.Contains("$ OK") ||
                s.Contains("$ NO"));

            ReleaseStream(i);
            _hSemaphore.Release();
            return a
                .Last()
                .Contains("$ OK");
        }

        /// <summary>
        /// Loads uids.
        /// </summary>
        public async Task LoadUids()
        {
            _hSemaphore.WaitOne();
            int i = GetFreeStream();

            await _streamOperators[i]
                .ReceiveResponse("$ SELECT INBOX", s => s.Contains("$ OK"));

            var response = await _streamOperators
                .ElementAt(i)
                .ReceiveResponse("$ UID SEARCH ALL", s => s.Contains("$ OK"));

            Regex regex = new Regex(@"(?<=SEARCH )((\d*) )*(\d*)");
            _uidsCash = regex.Match(
                string.Join(" ", response.Take(response.Count - 1)))
                .Value
                .Split(' ')
                .OrderByDescending(int.Parse)
                .ToList();

            ReleaseStream(i);
            _hSemaphore.Release();
        }

        /// <summary>
        /// Gets the current uids.
        /// </summary>
        private void GetCurrentUids()
        {
            _currentUids = _uidsCash.GetRange(MessagesLoaded, Step);
            MessagesLoaded += Step;
        }

        /// <summary>
        /// Updates the messages.
        /// </summary>
        /// <returns>
        /// List of message infomation entities
        /// </returns>
        public async Task<List<ImapMessageInfo>> UpdateMessages()
        {
            await LoadUids();
            _messagesCash.Clear();
            MessagesLoaded = 0;
            return await LoadMessages();
        }

        /// <summary>
        /// Loads the messages.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ImapMessageInfo>> LoadMessages()
        {
            GetCurrentUids();
            _messagesCash.Clear();

            var tasks = _currentUids.Select(item => Task.Run(async () =>
            {
                _hSemaphore.WaitOne();
                int i = GetFreeStream();

                try
                {
                    // select folder
                    await _streamOperators[i]
                        .ReceiveResponse("$ SELECT INBOX", s => s.Contains("$ OK"));

                    // fetch headers
                    var headerResult = await _streamOperators[i]
                        .ReceiveResponse($"$ UID FETCH {item} BODY[HEADER]", s => s.Contains("$ OK"));

                    // fetch subject field
                    var subjectResult = await _streamOperators[i]
                        .ReceiveResponse($"$ UID FETCH {item} BODY[HEADER.FIELDS (Subject)]", s => s.Contains("$ OK"));

                    // fetch body
                    var textResult = await _streamOperators[i]
                        .ReceiveResponse($"$ UID FETCH {item} BODY[TEXT]", s => s.Contains("$ OK"));
                    
                    var flags = await GetFlags(item, _streamOperators[i]);

                    // apply filters
                    var header = headerResult
                        .Where(n => !n.StartsWith("* ") && !n.StartsWith("$ ") && n != ")")
                        .ToList();

                    var subject = subjectResult
                        .Where(n => !n.StartsWith("* ") && !n.StartsWith("$ ") && n != ")")
                        .ToList();

                    var body = textResult
                      .Where(n => !n.StartsWith("* ") && !n.StartsWith("$ ") && n != ")")
                        .ToList();

                    var msg = ParseBodyHeader(item, header, string.Join("\r\n", subject));

                    // map to entity
                    var info = new ImapMessageInfo(msg)
                    {
                        Flags = flags
                    };

                    // load both valid text formats
                    info.Message.TextHtml = LoadText(header, body, "text/html");
                    info.Message.TextPlain = LoadText(header, body, "text/plain");

                    lock (_o)
                    {
                        _messagesCash.Add(info);
                    }

                }
                catch (Exception e)
                {
                    throw new ImapException(e.Message);
                }

                ReleaseStream(i);

                _hSemaphore.Release();
            })).ToArray();

            await Task.WhenAll(tasks);
            return _messagesCash
                .OrderByDescending(n => int.Parse(n.Message.Uid))
                .ToList();
        }

        /// <summary>
        /// Quits this session.
        /// </summary>
        public async Task Quit()
        {
            var tasks = new Task[_simultConnections];

            // logout all
            for (int i = 0; i < _simultConnections; i++)
            {
                int j = i;
                tasks[j] = Task.Run(async () =>
                {
                    try
                    {
                        await _streamOperators[j].ReceiveResponse("$ LOGOUT", s => s.Contains("BYE"));

                        _tcpClients[j].GetStream().Close();
                        _tcpClients[j].Close();
                        _streams[j].Close();
                    }
                    catch (Exception e)
                    {
                        throw new ImapException(e.Message);
                    }
                });
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Gets all flags.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="streamOperator1">The stream operator1.</param>
        /// <returns></returns>
        /// <exception cref="ImapException"></exception>
        private async Task<List<string>> GetFlags(string uid, StreamOperator streamOperator1)
        {
            var result = new List<string>();

            try
            {
                var response = await streamOperator1
                                .ReceiveResponse(
                        $"$ UID FETCH {uid} (FLAGS)",
                        s => s.Contains("$ OK")
                             || s.Contains("$ BAD")
                             || s.Contains("$ NO"));

                var regex = new Regex(@"\\(\w*)");
                var matches = regex.Matches(string.Join(" ", response));

                foreach (Match item in matches)
                {
                    result.Add(item.Value);
                }
            }
            catch (Exception e)
            {
                throw new ImapException(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Parses the body header.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="response">The response.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        private MessageModel ParseBodyHeader(string uid, List<string> response, string subject)
        {

            var regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            var email = new MessageModel(uid);

            var from = regex.Match((from line in response
                                    where line.StartsWith("From: ")
                                    select line)
                                .FirstOrDefault() ?? "")
                                .Value;

            var toMatches = regex.Matches((from line in response
                                           where line.StartsWith("To: ")
                                           select line)
                                           .FirstOrDefault() ?? "");

            var to = (from Match item in toMatches
                      select item.Value)
                      .ToList();

            var date = (from line in response
                        where line.StartsWith("Date: ")
                        select line.Replace("Date: ", string.Empty))
                        .FirstOrDefault();

            DateTime parsedDate = default(DateTime);
            if (date != null)
            {
                date = date.Contains("(")
                  ? date.Substring(0, date.Length - 5)
                  : date;
                parsedDate = Convert.ToDateTime(date);
            }

            var regexSubj = new Regex(@"(?<=Subject: )([\s\S]*?)(?=$)");

            string subj = regexSubj.Match(subject).Value;

            email.Subject = MessageDecoder.DecodeEncodedLine(subj);
            email.From = from;
            email.To = to;
            email.Date = parsedDate;

            return email;
        }

        /// <summary>
        /// Loads the text.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="body">The body.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private string LoadText(IReadOnlyCollection<string> header, List<string> body, string contentType)
        {
            var regexEncoding = new Regex(@"(?<=Content-Transfer-Encoding: )(\S*?)(?=$)");
            var regexType = new Regex(@"(?<=Content-Type: )(.*?)(?=(;))");

            if (string.Join(" ", header).Contains("Content-Transfer-Encoding: "))
            {
                var type = regexType.Match((from line in header
                                            where line.StartsWith($"Content-Type: {contentType}")
                                            select line).FirstOrDefault() ?? "")
                                          .Value;

                if (type == "")
                {
                    return $"Message Has No {contentType} Attached";
                }

                var commonEncoding = regexEncoding.Match((from line in header
                                                          where line.StartsWith("Content-Transfer-Encoding: ")
                                                          select line).FirstOrDefault() ?? "")
                                            .Value;

                var res = string.Join("\r\n", body);
                return MessageDecoder.DecodeBody(res, commonEncoding, "utf-8");
            }

            var matching = (from line in body
                            where line.StartsWith($"Content-Type: {contentType}")
                            select line).FirstOrDefault();

            if (matching == default(string))
            {
                return $"Message Has No {contentType} Attached";
            }

            var startOfHtml = body.IndexOf(matching) + 2;

            var boundary = (from line in body.Skip(startOfHtml)
                            where line.StartsWith("--") && line[2] != '>'
                            select line).FirstOrDefault();

            int endOfHtml = body.IndexOf(boundary, startOfHtml);

            var encoding = regexEncoding.Match(body[startOfHtml - 1]).Value;

            var result = string.Join("\r\n", body.GetRange(startOfHtml, endOfHtml - startOfHtml));

            string decodedBody = MessageDecoder.DecodeBody(result, encoding, "utf-8");
            return decodedBody;
        }

        /// <summary>
        /// Gets the free stream.
        /// </summary>
        /// <returns></returns>
        private int GetFreeStream()
        {
            lock (_o0)
            {
                return _freeStreams.Dequeue();
            }
        }

        /// <summary>
        /// Releases the stream.
        /// </summary>
        /// <param name="streamIndex">Index of the stream.</param>
        private void ReleaseStream(int streamIndex)
        {
            lock (_o1)
            {
                _freeStreams.Enqueue(streamIndex);
            }
        }

    }

}
