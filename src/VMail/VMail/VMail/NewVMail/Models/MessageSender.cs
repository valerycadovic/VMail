using Email.Smtp;
using Plugin.Settings;
using System.Threading.Tasks;

namespace VMail.NewVMail.Models
{
    public class MessageSender
    {
        string login;
        string password;
        string host;

        const string ehlo_str = "valera";

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSender"/> class.
        /// </summary>
        public MessageSender()
        {
            login = CrossSettings.Current.GetValueOrDefault(VMail.Models.User.address_cross_name, "");
            password = CrossSettings.Current.GetValueOrDefault(VMail.Models.User.password_cross_name, "");
            host = "smtp." + login.Substring(login.IndexOf("@") + 1);
        }

        /// <summary>
        /// Sends the specified subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="text">The text.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public async Task Send(string subject, string text, params string[] to)
        {
            using (SmtpConnection smtp = new SmtpConnection())
            {
                await smtp.Connect(host, 587);
                await smtp.Ehlo(ehlo_str);
                await smtp.StartTls(host);
                await smtp.Ehlo(ehlo_str);
                await smtp.AuthPlain(login, password);
                await smtp.Mail(login);
                foreach (var item in to)
                {
                    await smtp.Rcpt(item);
                }
                await smtp.Data(MessageFormatter.GetHtml(login, subject, text, to));
                await smtp.Quit();
            }
        }
    }
}
