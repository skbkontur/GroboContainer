using System;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public interface ICreationContext
    {
        IClassFactory BuildFactory(Type implementationType, Type[] parameterTypes);
    }
}