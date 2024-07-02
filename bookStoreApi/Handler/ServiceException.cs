using System.Net;

namespace bookStoreApi.Handler
{

    public class ServiceException : Exception
    {
        public ServiceException()
        {
        }

        public ServiceException(string? message) : base(message)
        {
        }

        public ServiceException(string? message, HttpStatusCode? code) : base(message)
        {
        }
    }
}
