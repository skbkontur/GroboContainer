using System;

namespace GroboContainer.Impl.Exceptions
{
    public class NoConstructorException : Exception
    {
        public NoConstructorException(Type type)
            : base($"Невозможно создать тип {type}")
        {
        }
    }
}