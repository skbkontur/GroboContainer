using System;

namespace GroboContainer.Impl.Exceptions
{
    public class AmbiguousConstructorException : Exception
    {
        public AmbiguousConstructorException(Type type) : base(string.Format("Невозможно создать тип {0}", type))
        {
        }
    }
}