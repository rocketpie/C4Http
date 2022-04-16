using C4Http.Interfaces;

namespace C4Http
{
    public class HttpClient
    {
        private readonly IRequestWriter _requestWriter;
        private readonly IResponseReader _responseReader;
        private readonly IConnectionProvider _connectionProvider;
        private readonly ITLSStreamProvider _tlsStreamProvider;

        public HttpClient(IRequestWriter? requestWriter = null, IResponseReader? responseReader = null, IConnectionProvider? connectionProvider = null, ITLSStreamProvider? tLSStreamProvider = null)
        {
            _requestWriter = requestWriter ?? new Default.RequestWriter();
            _responseReader = responseReader ?? new Default.ResponseReader();
            _connectionProvider = connectionProvider ?? new Default.ConnectionProvider();
            _tlsStreamProvider = tLSStreamProvider ?? new Default.TLSStreamProvider();
        }


        public async Task<ResponseContext> SendAsync(RequestContext context)
        {
            var tcpConnection = await _connectionProvider.GetConnectionAsync(context);

            var tlsConnection = await _tlsStreamProvider.WrapAsync(tcpConnection, context);

            await _requestWriter.WriteToStreamAsync(tlsConnection, context);

            return await _responseReader.ReadAsync(tlsConnection, context);
        }
    }
}