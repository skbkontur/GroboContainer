using System;

namespace GroboContainer.Infection
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RootTypeAttribute : Attribute
    {
    }
}