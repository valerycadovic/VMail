using System.IO;
using System.Threading.Tasks;

namespace Email
{
    public interface ITlsEncryptor
    {
        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="innerStream">The inner stream.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        Task<Stream> AuthenticateAsync(Stream innerStream, string host);
    }
}
