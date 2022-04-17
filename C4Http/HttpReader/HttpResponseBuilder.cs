using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace C4Http.HttpReader
{
    internal class HttpResponseBuilder
    {
        public NameValueCollection Headers { get; set; } = new NameValueCollection();
        public HttpStatusCode? StatusCode { get; set; }
        public string? Reason { get; set; }
        public StringBuilder BodyBuilder { get; set; } = new StringBuilder();
    }
}
