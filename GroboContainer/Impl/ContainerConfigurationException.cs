using System;

namespace GroboContainer.Impl
{
    public class ContainerConfigurationException : Exception
    {
        public ContainerConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}