namespace C4Http.HttpReader
{
    abstract class ReaderState
    {
        public HttpStreamReader ResponseReader { get; private set; }
        protected HttpResponseBuilder ResponseBuilder { get; private set; }

        protected ReaderState(HttpStreamReader responseReader, HttpResponseBuilder? responseBuilder = null)
        {
            ResponseReader = responseReader;
            ResponseBuilder = responseBuilder ?? new HttpResponseBuilder();
        }

        public abstract Task<bool> RunAsync();
        public ReaderState? NextState { get; protected set; }
    }
}
