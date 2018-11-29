namespace Email.Imap
{
    /// <summary>
    /// Imap exception class
    /// </summary>
    /// <seealso cref="Email.EmailException" />
    public class ImapException : EmailException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ImapException(string message = "") : base(message) { }
    }
}
