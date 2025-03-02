namespace PromptDetector.Exceptions
{
    public class InputValidationException : Exception
    {
        public InputValidationException()
        {
        }

        public InputValidationException(string message) : base(message)
        {
        }

        public InputValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
