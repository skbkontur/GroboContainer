using System;

using GroboContainer.Core;

namespace GroboContainer.Config
{
    public interface IAbstractionConfigurator
    {
        void UseInstances(params object[] instances);
        void Fail();
        void UseType(Type type);
        void UseTypes(Type[] types);
        void UseFactory(Func<IContainer, Type, object> factoryFunc);
    }
}