using System;

using GroboContainer.Core;

namespace GroboContainer.Config.Generic
{
    public interface IAbstractionConfigurator<in T>
    {
        void UseInstances(params T[] instances);
        void Fail();
        void UseType<TImpl>() where TImpl : T;
        void UseFactory<TImpl>(Func<IContainer, Type, TImpl> factoryFunc) where TImpl : T;
    }
}