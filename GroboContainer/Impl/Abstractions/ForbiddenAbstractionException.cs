using System;

namespace GroboContainer.Impl.Abstractions
{
    public class ForbiddenAbstractionException : Exception
    {
        public ForbiddenAbstractionException(Type type)
            : base($"Тип {type} запрещен")
        {
        }
    }
}