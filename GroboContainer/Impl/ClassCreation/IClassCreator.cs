using System;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IClassCreator
    {
        IClassFactory BuildFactory(ContainerConstructorInfo constructorInfo, Type wrapperType);
    }
}