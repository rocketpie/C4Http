using C4Http.Interfaces;
using System.Net.Sockets;

namespace C4Http.Default
{
    internal class ConnectionProvider : IConnectionProvider
    {
        public ConnectionProvider()
        {
        }

        public async Task<Stream> GetConnectionAsync(RequestContext context)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(context.Url.DnsSafeHost, context.Url.Port);
            return tcpClient.GetStream();
        }
    }
}