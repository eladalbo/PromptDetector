using System.Runtime.Serialization;

namespace PromptDetector.Exceptions
{
    public class ExternalCallException : Exception
    {
        public ExternalCallException()
        {
        }

        public ExternalCallException(string? message) : base(message)
        {
        }

        public ExternalCallException(string? message, Exception? innerException) : base(message, innerException)
        {
        }      
    }
}
