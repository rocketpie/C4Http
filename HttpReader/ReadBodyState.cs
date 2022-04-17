namespace C4Http.HttpReader
{
    internal class ReadBodyState : ReaderState
    {
        private readonly int _contentLength;

        public ReadBodyState(HttpStreamReader responseReader, HttpResponseBuilder responseBuilder, int contentLength) : base(responseReader, responseBuilder)
        {
            _contentLength = contentLength;
        }

        public override async Task<bool> RunAsync()
        {
            (var readSize, var body) = await ResponseReader.ReadBytesAsync(_contentLength);
            ResponseBuilder.BodyBuilder.Append(body);

            if (readSize != _contentLength)
            {
                throw new Exception($"a828a2 incomplete response: {readSize}/{_contentLength} bytes read");
            }

            return true;
        }
    }
}
