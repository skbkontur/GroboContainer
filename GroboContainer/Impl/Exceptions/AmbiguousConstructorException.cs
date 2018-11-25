using System;

namespace GroboContainer.Impl.Exceptions
{
    public class AmbiguousConstructorException : Exception
    {
        public AmbiguousConstructorException(Type type)
            : base($"Невозможно создать тип {type}")
        {
        }
    }
}