using System.Net;
using System.Text.RegularExpressions;

namespace C4Http.HttpReader
{
    internal class ReadStatusLineState : ReaderState
    {
        public ReadStatusLineState(HttpStreamReader responseReader) : base(responseReader) { }
        public override async Task<bool> RunAsync()
        {
            var statusLine = await ResponseReader.ReadLineAsync();
            var match = Regex.Match(statusLine, @"HTTP/1.1 (\d{1,3}) (\w+)");

            if (!match.Success)
            {
                return false;
            }

            ResponseBuilder.StatusCode = (HttpStatusCode)int.Parse(match.Groups[1].Value);
            ResponseBuilder.Reason = match.Groups[2].Value;
            NextState = new ReadHeaderState(ResponseReader, ResponseBuilder);
            return true;
        }
    }
}
