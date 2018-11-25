using System;

namespace GroboContainer.Impl.Exceptions
{
    public class InaccessibleTypeException : Exception
    {
        public InaccessibleTypeException(Type type)
            : base($"Тип {type} объявлен непубличным")
        {
        }
    }
}