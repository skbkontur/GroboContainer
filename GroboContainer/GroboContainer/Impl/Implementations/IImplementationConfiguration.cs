using System;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public interface IImplementationConfiguration
    {
        Type ObjectType { get; }
        object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext);
        void DisposeInstance();
        IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext);
    }
}