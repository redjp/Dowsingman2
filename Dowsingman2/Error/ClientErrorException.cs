using System;
using System.Runtime.Serialization;

namespace Dowsingman2.Error
{
    internal class ClientErrorException : HttpClientException
    {
        public ClientErrorException()
        {
        }

        public ClientErrorException(string message)
            : base(message)
        {
        }

        protected ClientErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ClientErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}