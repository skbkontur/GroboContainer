using System;

namespace GroboContainer.Infection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoredImplementationAttribute : Attribute
    {
    }
}