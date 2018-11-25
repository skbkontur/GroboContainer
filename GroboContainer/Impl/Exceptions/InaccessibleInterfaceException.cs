using System;

namespace GroboContainer.Impl.Exceptions
{
    public class InaccessibleInterfaceException : Exception
    {
        public InaccessibleInterfaceException(Type type)
            : base($"Интерфейс {type} объявлен непубличным")
        {
        }
    }
}