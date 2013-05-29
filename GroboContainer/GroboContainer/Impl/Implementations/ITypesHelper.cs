using System;
using System.Reflection;

namespace GroboContainer.Impl.Implementations
{
    public interface ITypesHelper
    {
        Type TryGetImplementation(Type abstractionType, Type candidate);
        bool IsIgnoredImplementation(ICustomAttributeProvider provider);
        bool IsIgnoredAbstraction(ICustomAttributeProvider provider);
    }
}