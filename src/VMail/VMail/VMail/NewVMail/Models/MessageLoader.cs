using Email;
using Email.Imap;
using System.Collections.Generic;
using Plugin.Settings;
using VMail.Models;
using System.Threading.Tasks;

namespace VMail.NewVMail.Models
{
    public class MessageLoader
    {
        private List<MessageModel> _messages;
        
        private readonly ImapClient _imap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageLoader"/> class.
        /// </summary>
        public MessageLoader()
        {
            _messages = new List<MessageModel>();
            _imap = new ImapClient();
        }

        /// <summary>
        /// Gets the messages count.
        /// </summary>
        /// <value>
        /// The messages count.
        /// </value>
        public int MessagesCount => _imap.MessagesCount;

        /// <summary>
        /// Gets the messages loaded.
        /// </summary>
        /// <value>
        /// The messages loaded.
        /// </value>
        public int MessagesLoaded => _imap.MessagesLoaded;

        /// <summary>
        /// Logins this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Login()
        {
            string address = CrossSettings.Current.GetValueOrDefault(User.address_cross_name, "");
            string password = CrossSettings.Current.GetValueOrDefault(User.password_cross_name, "");

            await _imap.ConnectAsync(address, password);
        }

        /// <summary>
        /// Loads the messages.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ImapMessageInfo>> LoadMessages()
        {
            await _imap.SelectFolder("INBOX");  
            return await _imap.LoadMessages();
        }

        /// <summary>
        /// Updates the messages.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ImapMessageInfo>> UpdateMessages()
        {
            await _imap.SelectFolder("INBOX");  
            return await _imap.UpdateMessages();
        }

        /// <summary>
        /// Sees the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public async Task SeeMessage(ImapMessageInfo msg)
        {
            await _imap.SelectFolder("INBOX");
            await _imap.SetFlag(msg.Message.Uid, '+', @"\Seen");
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        public async Task DeleteMessage(ImapMessageInfo msg)
        {
            await _imap.SelectFolder("INBOX");
            await _imap.SetFlag(msg.Message.Uid, '+', @"\Deleted");
        }
    }
}
