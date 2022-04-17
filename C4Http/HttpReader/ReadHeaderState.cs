using System.Collections.Specialized;

namespace C4Http.HttpReader
{
    internal class ReadHeaderState : ReaderState
    {
        public ReadHeaderState(HttpStreamReader responseReader, HttpResponseBuilder responseBuilder) : base(responseReader, responseBuilder) { }

        public override async Task<bool> RunAsync()
        {
            string? headerLine;
            do
            {
                headerLine = await ResponseReader.ReadLineAsync();
                ParseAndAdd(headerLine, ResponseBuilder.Headers);
            } while (!string.IsNullOrWhiteSpace(headerLine));

            var contentLengthString = ResponseBuilder.Headers.Get("Content-Length");
            var transferEncoding = ResponseBuilder.Headers.Get("Transfer-Encoding");

            if (int.TryParse(contentLengthString, out var contentLength))
            {
                NextState = new ReadBodyState(ResponseReader, ResponseBuilder, contentLength);
                return true;
            }

            if (transferEncoding != "chunked")
            {
                throw new Exception("82c1ba neither Content-Length nor Transfer-Encoding:chunked was provided");
            }

            var chunkSizeLine = await ResponseReader.ReadLineAsync();
            if (int.TryParse(chunkSizeLine, System.Globalization.NumberStyles.HexNumber, null, out var chunkSize))
            {
                NextState = new ReadChunkedState(ResponseReader, ResponseBuilder, chunkSize);
                return true;
            }

            throw new Exception("eea3ad neither Content-Length nor Transfer-Encoding:chunked was provided");
        }

        private static void ParseAndAdd(string? headerLine, NameValueCollection headers)
        {
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                return;
            }

            if (!headerLine.Contains(':')) { throw new Exception($"54d9ed invalid header '{headerLine}'"); }
            var data = headerLine.Split(':');

            headers.Add(data[0].Trim(), data[1].Trim());
        }
    }
}
