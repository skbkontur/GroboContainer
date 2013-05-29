using System;

namespace GroboContainer.Infection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class IgnoredAbstractionAttribute : Attribute
    {
    }
}