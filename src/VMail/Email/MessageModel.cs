using System;
using System.Collections.Generic;

namespace Email
{
    public class MessageModel
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the text plain.
        /// </summary>
        public string TextPlain { get; set; }

        /// <summary>
        /// Gets or sets the text HTML.
        /// </summary>
        public string TextHtml { get; set; }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        public string Uid { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageModel"/> class.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public MessageModel(string uid)
        {
            Uid = uid;
        }
    }
}
