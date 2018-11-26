using System;
using System.Diagnostics.CodeAnalysis;

namespace GroboContainer.Infection
{
    [SuppressMessage("ReSharper", "RedundantAttributeUsageProperty")]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChildTypeAttribute : Attribute
    {
    }
}