using System;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IConstructorSelector
    {
        ContainerConstructorInfo GetConstructor(Type source, params Type[] parameterTypes);
    }
}