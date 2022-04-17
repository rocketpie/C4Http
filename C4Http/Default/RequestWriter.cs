using C4Http.Interfaces;
using System.Text;

namespace C4Http.Default
{
    internal class RequestWriter : IRequestWriter
    {
        public async Task WriteToStreamAsync(Stream httpStream, RequestContext context)
        {
            var writer = new StreamWriter(httpStream, new UTF8Encoding(false));
            writer.AutoFlush = false;

            writer.Write(context.Method);
            writer.Write(" ");
            writer.Write(context.Url.PathAndQuery);
            writer.Write(" ");
            writer.Write("HTTP/1.1");
            writer.Write("\r\n");
            writer.Write($"Host: {context.Url.Host}");
            foreach (var key in context.RequestHeaders.Keys.Cast<string>())
            {
                writer.Write("\r\n");
                writer.Write(key);
                writer.Write(':');
                writer.Write(context.RequestHeaders[key]);
            }
            writer.Write("\r\n");
            writer.Write("\r\n");

            if (context.Body != null)
            {
                writer.Write(context.Body);
            }
            else if (context.GetBody != null)
            {
                writer.Write(context.GetBody());
            }
            else if (context.BodyWriter != null)
            {
                await context.BodyWriter(context, writer);
            }

            writer.Write("\r\n");
            await writer.FlushAsync();
        }
    }
}
