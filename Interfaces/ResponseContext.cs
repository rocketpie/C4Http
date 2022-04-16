using System.Collections.Specialized;
using System.Net;

namespace C4Http.Interfaces
{
    public class ResponseContext
    {
        const HttpStatusCode REDIRECT_CODES = HttpStatusCode.Moved | HttpStatusCode.Found | HttpStatusCode.TemporaryRedirect | HttpStatusCode.PermanentRedirect;

        /// <summary>
        /// a response has been returned and was successfully read
        /// </summary>
        public bool IsRequestSuccess => Exception == null;
        public bool IsRedirect => Exception == null && ((REDIRECT_CODES & StatusCode) == StatusCode);

        public Exception? Exception { get; internal set; }

        public DateTime? RequestFirstByteSent { get; internal set; }
        public DateTime? RequestSent { get; internal set; }
        public DateTime? ResponseFirstByteReceived { get; internal set; }
        public DateTime? ResponseReceived { get; internal set; }


        private HttpStatusCode? _statusCode;
        public HttpStatusCode StatusCode
        {
            get
            {
                if (Exception != null) { throw Exception; }
                if (_statusCode == null) { throw new InvalidOperationException("c9480ac why is there no exception?"); }
                return _statusCode.Value;
            }
            internal set { _statusCode = value; }
        }

        NameValueCollection? _responseHeaders;
        public NameValueCollection ResponseHeaders
        {
            get
            {
                if (Exception != null) { throw Exception; }
                if (_responseHeaders == null) { throw new InvalidOperationException("c9480ad why is there no exception?"); }
                return _responseHeaders;
            }
            internal set { _responseHeaders = value; }
        }

        StreamReader? _bodyStream;
        public StreamReader BodyStream
        {
            get
            {
                if (Exception != null) { throw Exception; }
                if (_bodyStream == null) { throw new InvalidOperationException("c9480ae why is there no exception?"); }
                return _bodyStream;
            }
            internal set { _bodyStream = value; }
        }

    }
}
