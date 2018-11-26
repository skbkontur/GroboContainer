using System;

namespace GroboContainer.Impl.Exceptions
{
    public class NoImplementationException : Exception
    {
        public NoImplementationException(Type requested)
            : base($"Type {requested} has no implementations")
        {
        }
    }
}