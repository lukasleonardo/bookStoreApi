using System.Net;

namespace bookStoreApi.Handler
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string? message) : base(message)
        {
        }
        public NotFoundException(string? message, HttpStatusCode? code) : base(message)
        {
        }
    }
}
