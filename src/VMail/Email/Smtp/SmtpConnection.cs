using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Email.Smtp
{
    public class SmtpConnection : IDisposable
    {
        #region Private Fields

        private StreamOperator _streamOperator;
        private TcpClient _tcp;
        private readonly ITlsEncryptor _tls;
        private Stream _stream;
        private readonly List<string> _commandsAvailable;
        private const string DataEnd = "\r\n.\r\n";
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpConnection"/> class.
        /// </summary>
        /// <param name="tls">The TLS.</param>
        /// <exception cref="ArgumentNullException">SmtpConnection</exception>
        public SmtpConnection(ITlsEncryptor tls)
        {
            _tls = tls ?? throw new ArgumentNullException(nameof(SmtpConnection) + " Null argument");
            _commandsAvailable = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpConnection"/> class.
        /// </summary>
        public SmtpConnection() : this(new DefaultTlsEncryptor()) { }
        #endregion

        #region SMTP Calls        
        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public async Task Connect(string host, int port = 587)
        {
            _tcp = new TcpClient(host, port);
            _stream = _tcp.GetStream();
            _streamOperator = new EmailStreamOperator(_stream);

            await ReceiveResponse("");             // set first connection
        }

        /// <summary>
        /// Ehloes the specified hello.
        /// </summary>
        /// <param name="hello">The hello.</param>
        /// <returns></returns>
        public async Task Ehlo(string hello)
        {
            var response = await ReceiveResponse($"EHLO {hello}");
            ParseEhlo(response);
        }

        /// <summary>
        /// Starts the TLS.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        /// <exception cref="SmtpException">STARTTLS is not available</exception>
        /// <exception cref="Exception">Smtp Servise is not ready <{nameof(StartTls)}</exception>
        public async Task StartTls(string host)
        {
            if (!_commandsAvailable.Contains("STARTTLS"))
                throw new SmtpException("STARTTLS is not available");

            var response = await ReceiveResponse("STARTTLS");
            if (ParseCode(response) != "220") throw new Exception($"Smtp Servise is not ready <{nameof(StartTls)}>");

            _stream = await _tls.AuthenticateAsync(_stream, host);
            _streamOperator = new EmailStreamOperator(_stream);
        }

        /// <summary>
        /// Authentications the plain.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="SmtpException">
        /// Not supported
        /// or
        /// Invalid credentials
        /// </exception>
        /// <exception cref="Exception">Unknown Exception</exception>
        public async Task AuthPlain(string login, string password)
        {
            if (!_commandsAvailable.Contains("AUTH PLAIN"))
                throw new SmtpException("Not supported");

            var reply = await ReceiveResponse($"AUTH PLAIN {EncodePlainToBase64(string.Empty, login, password)}");

            switch (ParseCode(reply))
            {
                case "235":
                    return;
                case "535":
                    throw new SmtpException("Invalid credentials");
                default:
                    throw new Exception("Unknown Exception");
            }
        }

        /// <summary>
        /// Sets sender address
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        /// <exception cref="SmtpException">Mail</exception>
        public async Task Mail(string from)
        {
            var response = await ReceiveResponse($"MAIL FROM:<{from}>");

            if (ParseCode(response) != "250")
                throw new SmtpException(nameof(Mail));
        }

        /// <summary>
        /// sets recipient address
        /// </summary>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <exception cref="SmtpException">Rcpt</exception>
        public async Task Rcpt(string to)
        {
            var response = await ReceiveResponse($"RCPT TO:<{to}>");
            if (ParseCode(response) != "250")
                throw new SmtpException(nameof(Rcpt));
        }

        /// <summary>
        /// Defines data part
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns></returns>
        /// <exception cref="SmtpException">
        /// Data
        /// or
        /// </exception>
        public async Task Data(string mail)
        {
            var response = await ReceiveResponse("DATA");

            if (mail.Contains(DataEnd))
                throw new SmtpException($"{nameof(Data)}: {nameof(mail)} cannot contain <CRLF>.<CRLF> 'cause it have to be the end of message terminator");

            if (ParseCode(response) != "354")
                throw new SmtpException(nameof(Data));



            var dataResponse = await ReceiveResponse(mail + DataEnd);

            if (ParseCode(dataResponse) != "250")
                throw new SmtpException($"{nameof(Data)}: error while sending message");

        }

        /// <summary>
        /// Quits this connection.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SmtpException"></exception>
        public async Task Quit()
        {
            var response = await ReceiveResponse("QUIT");
            if (ParseCode(response) != "221")
                throw new SmtpException($"{nameof(Quit)}: illegal quit");
        }
        #endregion

        #region Private Section        
        /// <summary>
        /// Encodes the plain to base64.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <param name="authentication">The authentication.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private static string EncodePlainToBase64(string authorization, string authentication, string password)
        {
            string msg = authorization + '\0' + authentication + '\0' + password;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
        }

        /// <summary>
        /// Parses the ehlo.
        /// </summary>
        /// <param name="response">The response.</param>
        private void ParseEhlo(IEnumerable<string> response)
        {
            _commandsAvailable.Clear();

            foreach (var line in response.Skip(1))
            {
                if (line.Contains("STARTTLS"))
                {
                    _commandsAvailable.Add(line.Substring(4));
                    continue;
                }

                var words = line.Substring(4).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0)
                    continue;

                if (words[0] == "AUTH")
                {
                    foreach (var type in words.Skip(1))
                        _commandsAvailable.Add($"AUTH {type}");
                }
            }
        }

        /// <summary>
        /// Parses the code.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">
        /// Wrong response format <{nameof(ParseCode)}
        /// or
        /// Wrong code <{nameof(ParseCode)}
        /// </exception>
        private string ParseCode(List<string> response)
        {
            var regex = new Regex(@"^\d{3}(?=(-|\s))");
            var codes = new HashSet<string>();
            foreach (var line in response)
            {
                if (regex.IsMatch(line))
                    codes.Add(regex.Match(line).Value);
                else throw new FormatException($"Wrong response format <{nameof(ParseCode)}>");
            }

            if (codes.Count == 1)
                return codes.First();

            throw new FormatException($"Wrong code <{nameof(ParseCode)}>");
        }

        // last line identifier is common for all SMTP requests unlike IMAP
        private async Task<List<string>> ReceiveResponse(string request)
        { 
            return await _streamOperator.ReceiveResponse(request, s => true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _tcp.GetStream().Close();
            _tcp.Close();
            _stream.Close();
        }
        #endregion
    }

}
