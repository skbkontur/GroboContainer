using System;

using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IClassCreator
    {
        Func<IInternalContainer, IInjectionContext, object[], object> BuildConstructionDelegate(ContainerConstructorInfo constructorInfo, Type wrapperType);
        IClassFactory BuildFactory(ContainerConstructorInfo constructorInfo, Type wrapperType);
    }
}