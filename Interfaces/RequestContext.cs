using System.Collections.Specialized;

namespace C4Http.Interfaces
{
    public class RequestContext
    {
        public RequestContext(string method, Uri url, string requestBody)
        {
            Method = method;
            Url = url;
            Body = requestBody;
        }

        public Uri Url { get; set; }
        public string Method { get; set; }        
        public NameValueCollection RequestHeaders { get; set; } = new NameValueCollection();
        /// <summary>
        /// short Bodies may be provided directly
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// serialization effort may be executed lazy for larger bodies
        /// </summary>
        public Func<string>? GetBody { get; set; }
        /// <summary>
        /// even larger data may be streamed during Send()
        /// </summary>
        public Func<RequestContext, StreamWriter, Task>? BodyWriter { get; set; }



    }
}
