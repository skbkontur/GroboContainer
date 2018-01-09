using System;

namespace GroboContainer.Impl.Exceptions
{
    public class InaccessibleTypeException: Exception
    {
        public InaccessibleTypeException(Type type): base(string.Format("Тип {0} объявлен непубличным", type))
        {
        }
    }
}