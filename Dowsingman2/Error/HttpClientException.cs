using System;
using System.Runtime.Serialization;


namespace Dowsingman2.Error
{
    internal class HttpClientException : ApplicationException
    {
        public HttpClientException()
        {
        }

        public HttpClientException(string message)
            : base(message)
        {
        }

        protected HttpClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}