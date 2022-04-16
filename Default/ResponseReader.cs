using C4Http.Interfaces;

namespace C4Http.Default
{
    internal class ResponseReader : IResponseReader
    {
        public Task<ResponseContext> ReadAsync(Stream tlsConnection, RequestContext context)
        {
            throw new NotImplementedException();
        }
    }
}
