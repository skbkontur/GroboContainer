using System;

namespace GroboContainer.Impl.Injection
{
    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string lastConstructionLog) : base("\r\n" + lastConstructionLog)
        {
        }
    }
}