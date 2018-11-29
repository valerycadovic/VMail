using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

namespace Email
{
    public class DefaultTlsEncryptor : ITlsEncryptor
    {
        /// <summary>
        /// Authenticates asynchronous.
        /// </summary>
        /// <param name="innerStream">The inner stream.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public async Task<Stream> AuthenticateAsync(Stream innerStream, string host)
        {
            var ssl = new SslStream(innerStream, true);
            await ssl.AuthenticateAsClientAsync(host);

            return ssl;
        }
    }
}
