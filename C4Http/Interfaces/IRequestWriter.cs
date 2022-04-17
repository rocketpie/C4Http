namespace C4Http.Interfaces
{
    public interface IRequestWriter
    {
       Task WriteToStreamAsync(Stream httpStream, RequestContext context);
    }
}
