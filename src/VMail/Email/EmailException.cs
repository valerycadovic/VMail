using System;
using System.Collections.Generic;
using System.Text;

namespace Email
{
    public class EmailException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EmailException(string message = "") : base(message) { }
    }
}
