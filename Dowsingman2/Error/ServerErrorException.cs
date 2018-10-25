using System;
using System.Runtime.Serialization;

namespace Dowsingman2.Error
{
    internal class ServerErrorException : HttpClientException
    {
        public ServerErrorException()
        {
        }

        public ServerErrorException(string message)
            : base(message)
        {
        }

        protected ServerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ServerErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}