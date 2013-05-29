using System;

namespace GroboContainer.Impl.Exceptions
{
    public class ContainerException : Exception
    {
        public ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}