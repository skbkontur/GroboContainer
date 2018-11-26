using System;

namespace GroboContainer.Impl.Exceptions
{
    public class NoConstructorException : Exception
    {
        public NoConstructorException(Type type)
            : base($"No suitable constructor is found to instantiate type {type}")
        {
        }
    }
}