using System.Net.Sockets;

namespace C4Http.Interfaces
{
    public interface IConnectionProvider
    {
       Task<Stream> GetConnectionAsync(RequestContext context);
    }
}