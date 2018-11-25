using System;
using System.Diagnostics.CodeAnalysis;

namespace GroboContainer.Infection
{
    [SuppressMessage("ReSharper", "RedundantAttributeUsageProperty")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class IgnoredAbstractionAttribute : Attribute
    {
    }
}