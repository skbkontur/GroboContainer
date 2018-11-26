using System;

namespace GroboContainer.Impl.Exceptions
{
    public class AmbiguousConstructorException : Exception
    {
        public AmbiguousConstructorException(Type type)
            : base($"Cannot choose between several suitable constructors to instantiate type {type}")
        {
        }
    }
}