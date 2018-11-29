using System;
using System.Collections.Generic;
using System.Text;

namespace Email.Smtp
{
    public class SmtpException : EmailException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SmtpException(string message = "") : base(message) { }
    }
}
