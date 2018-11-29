using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Email
{
    internal abstract class StreamOperator
    {
        /// <summary>
        /// The stream
        /// </summary>
        protected Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamOperator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected StreamOperator(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Receives the response.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="isLastLine">The is last line.</param>
        /// <returns></returns>
        public abstract Task<List<string>> ReceiveResponse(string query, Func<string, bool> isLastLine);
    }
}
