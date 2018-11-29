using System;
using System.Collections.Generic;
using System.Text;

namespace EmailProtocols.Smtp.Realization
{
    public class SmtpException : Exception
    {
        public SmtpException(string message) : base(message) { }

        public SmtpException(string message, System.Exception inner) : base(message, inner) { }
    }
}
