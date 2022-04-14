using C4Http.Default;

namespace C4Http
{
    public class HttpClient
    {
        private RequestWriter _requestWriter;
        private ResponseReader _responseReader;

        public HttpClient()
        {
            _requestWriter = new Default.RequestWriter();
            _responseReader = new Default.ResponseReader();
        }
    }
}