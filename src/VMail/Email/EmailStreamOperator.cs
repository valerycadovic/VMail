using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    internal class EmailStreamOperator : StreamOperator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailStreamOperator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public EmailStreamOperator(Stream stream) : base(stream) { }

        /// <summary>
        /// Receives the response.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="isLastLine">The is last line.</param>
        /// <returns></returns>
        /// <exception cref="EndOfStreamException">Error while reading</exception>
        public override async Task<List<string>> ReceiveResponse(string query, Func<string, bool> isLastLine)
        {
            if (query != string.Empty)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(query + "\r\n");
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }

            var sb = new StringBuilder();
            string @string;
            do
            {
                var buffer = new byte[2048];
                int bytesGot = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesGot == 0)
                {
                    throw new EndOfStreamException("Error while reading");
                }
                @string = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
                sb.Append(@string);
            } while (!isLastLine(@string));

            return sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}

