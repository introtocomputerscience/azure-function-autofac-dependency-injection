using System;

namespace AzureFunctions.Autofac.Exceptions
{
    public class InitializationException : Exception
    {
        public InitializationException() : base() { }
        public InitializationException(string message) : base(message) { }
        public InitializationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
