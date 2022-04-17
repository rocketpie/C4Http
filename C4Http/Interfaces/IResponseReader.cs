namespace C4Http.Interfaces
{
    public interface IResponseReader
    {
        Task<ResponseContext> ReadAsync(Stream tlsConnection, RequestContext context);
    }
}
