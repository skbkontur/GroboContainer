using System;
using System.Diagnostics.CodeAnalysis;

namespace GroboContainer.Infection
{
    [SuppressMessage("ReSharper", "RedundantAttributeUsageProperty")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoredImplementationAttribute : Attribute
    {
    }
}