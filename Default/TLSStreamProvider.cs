using C4Http.Interfaces;
using System.Net.Security;

namespace C4Http.Default
{
    internal class TLSStreamProvider : ITLSStreamProvider
    {
        public TLSStreamProvider()
        {
        }

        public async Task<Stream> WrapAsync(Stream httpStream, RequestContext context)
        {
            var tlsStream = new SslStream(httpStream);
            await tlsStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions() { TargetHost = context.Url.Host });
            return tlsStream;
        }
    }
}