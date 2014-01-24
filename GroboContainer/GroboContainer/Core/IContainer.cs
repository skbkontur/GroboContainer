using System;
using GroboContainer.Config;

namespace GroboContainer.Core
{
    public interface IContainer : IDisposable
    {
        string Name { get; }
        IContainerConfigurator Configurator { get; }
        string LastConstructionLog { get; }
        IContainer MakeChildContainer();
        T Get<T>();
        object Get(Type abstractionType);
        T[] GetAll<T>();
        object[] GetAll(Type abstractionType);

        T Create<T>();
        T Create<T1, T>(T1 arg1);
        T Create<T1, T2, T>(T1 arg1, T2 arg2);
        T Create<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3);
        T Create<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

        object Create(Type abstractionType);
        object Create(Type abstractionType, Type[] parameterTypes, object[] parameters);

        Type[] GetImplementationTypes(Type abstractionType);
    }
}