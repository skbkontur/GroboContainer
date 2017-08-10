using System;
using System.Collections.Generic;
using System.Reflection;

namespace GroboContainer.Impl.Implementations
{
    public interface ITypesHelper
    {
        Type TryGetImplementation(Type abstractionType, Type candidate);
        Type TryGetImplementation(Type abstractionType, Type candidate, Func<Type, IEnumerable<Type>> getImplementations);
        bool IsIgnoredImplementation(ICustomAttributeProvider provider);
        bool IsIgnoredAbstraction(ICustomAttributeProvider provider);
    }
}