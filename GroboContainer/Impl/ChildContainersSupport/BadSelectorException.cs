using System;

namespace GroboContainer.Impl.ChildContainersSupport
{
    public class BadSelectorException : Exception
    {
        public BadSelectorException(string message)
            : base(message)
        {
        }
    }
}