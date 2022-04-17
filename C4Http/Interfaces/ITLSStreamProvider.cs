namespace C4Http.Interfaces
{
    public interface ITLSStreamProvider
    {
        Task<Stream> WrapAsync(Stream tcpStream, RequestContext context);
    }
}