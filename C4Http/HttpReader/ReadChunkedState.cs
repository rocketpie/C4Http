namespace C4Http.HttpReader
{
    internal class ReadChunkedState : ReaderState
    {
        private readonly int _chunkSize;

        public ReadChunkedState(HttpStreamReader responseReader, HttpResponseBuilder responseBuilder, int chunkSize) : base(responseReader, responseBuilder)
        {
            _chunkSize = chunkSize;
        }

        public override async Task<bool> RunAsync()
        {
            (var readSize, var chunk) = await ResponseReader.ReadBytesAsync(_chunkSize);
            ResponseBuilder.BodyBuilder.Append(chunk);

            if (readSize != _chunkSize)
            {
                throw new Exception($"a02129 incomplete chunk: {readSize}/{_chunkSize} bytes read");
            }

            var endOfChunk = await ResponseReader.ReadLineAsync();
            if (endOfChunk.Trim() != "")
            {
                throw new Exception($"a02130 incomplete chunk: found {endOfChunk.Length} chars instead of an empty line");
            }

            var nextChunkSize = await ResponseReader.ReadLineAsync();
            if (nextChunkSize.Trim() == "0")
            {
                // empty newline at the end of all chunk
                await ResponseReader.ReadLineAsync();
                NextState = new ReadCompletedState(ResponseReader, ResponseBuilder);
                return true;
            }

            if (int.TryParse(nextChunkSize, System.Globalization.NumberStyles.HexNumber, null, out var chunkSize))
            {
                NextState = new ReadChunkedState(ResponseReader, ResponseBuilder, chunkSize);
                return true;
            }

            throw new Exception("eea3ad cannot read next chunk size");
        }

    }
}

