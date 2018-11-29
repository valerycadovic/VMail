using System.Collections.Generic;

namespace Email.Imap
{
    /// <summary>
    /// imap headers entity
    /// </summary>
    public class ImapMessageInfo
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public MessageModel Message { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is read.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is read; otherwise, <c>false</c>.
        /// </value>
        public bool IsRead { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImapMessageInfo"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ImapMessageInfo(MessageModel message)
        {
            Message = message;
            Flags = new List<string>();
        }
        
        private List<string> _flags;

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public List<string> Flags
        {
            get => _flags;
            set
            {
                if (value.Contains(@"\Seen"))
                {
                    IsRead = true;
                }
                _flags = value;
            }
        }

        /// <summary>
        /// Determines whether the specified flag has flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns>
        ///   <c>true</c> if the specified flag has flag; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFlag(string flag)
        {
            return Flags.Contains(flag);
        }

        /// <summary>
        /// Removes the flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public bool RemoveFlag(string flag)
        {
            if (flag == @"\Seen")
            {
                IsRead = false;
            }

            return Flags.Remove(flag);
        }

        /// <summary>
        /// Adds the flag.
        /// </summary>
        /// <param name="flag">The flag.</param>
        public void AddFlag(string flag)
        {
            if (flag == @"\Seen")
            {
                IsRead = true;
            }

            if (!HasFlag(flag))
            {
                Flags.Add(flag);
            }
        }
    }
}
