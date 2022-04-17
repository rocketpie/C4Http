using C4Http.Interfaces;

namespace C4Http.HttpReader
{
    internal class ReadCompletedState : ReaderState
    {
        public ReadCompletedState(HttpStreamReader responseReader, HttpResponseBuilder responseBuilder) : base(responseReader, responseBuilder) { }
        public override Task<bool> RunAsync()
        {
            return Task.FromResult(false);
        }

        public ResponseContext GetResponseContext()
        {
            if (ResponseBuilder.StatusCode == null) {
                return new ResponseContext(new Exception("82659f"));
            }

            return new ResponseContext(ResponseBuilder.StatusCode.Value, ResponseBuilder.Headers, ResponseBuilder.BodyBuilder.ToString());
        }
    }
}
