using C4Http.HttpReader;
using C4Http.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace C4Http.Default
{
    internal class ResponseReader : IResponseReader
    {
        public async Task<ResponseContext> ReadAsync(Stream tlsConnection, RequestContext context)
        {
            ReaderState state = new ReadStatusLineState(new HttpStreamReader(tlsConnection, Encoding.UTF8));
            try
            {
                while (await state.RunAsync())
                {
                    if (state.NextState != null)
                    {
                        state = state.NextState;
                    }
                }
            }
            //                                                       the connection has been closed by the server    and there is no incomplete response left to be read.
            catch (IOException ioEx) when (ioEx.InnerException is SocketException socketEx && socketEx.NativeErrorCode == 10054 && state is ReadStatusLineState)
            {
                return new ResponseContext(new Exception("4e70e9 server hung up (10054) before a/another response was sent.", ioEx));
            }

            if (state is ReadCompletedState completed)
            {
                return completed.GetResponseContext();
            }

            return new ResponseContext(new Exception("4e70f2 server hung up (10054) before a/another response was sent."));
        }
    }
}
