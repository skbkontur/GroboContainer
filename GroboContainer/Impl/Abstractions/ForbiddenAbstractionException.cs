using System;

namespace GroboContainer.Impl.Abstractions
{
    public class ForbiddenAbstractionException : Exception
    {
        public ForbiddenAbstractionException(Type type)
            : base($"Type {type} is forbidden")
        {
        }
    }
}