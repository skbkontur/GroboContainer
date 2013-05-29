using System;
using GroboContainer.Config;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl
{
    public interface IInternalContainer : IFuncBuilder
    {
        IInternalContainer MakeChild();
        IContainerConfigurator Configurator { get; }
        void CallDispose();

        T Get<T>(IInjectionContext context);
        object Get(Type abstractionType, IInjectionContext context);

        T[] GetAll<T>(IInjectionContext context);
        object[] GetAll(Type abstractionType, IInjectionContext context);

        T Create<T>(IInjectionContext context);
        T Create<T1, T>(IInjectionContext context, T1 arg1);
        T Create<T1, T2, T>(IInjectionContext context, T1 arg1, T2 arg2);
        T Create<T1, T2, T3, T>(IInjectionContext context, T1 arg1, T2 arg2, T3 arg3);

        T Create<T1, T2, T3, T4, T>(IInjectionContext context, T1 arg1, T2 arg2, T3 arg3,
                                    T4 arg4);

        object Create(Type abstractionType, IInjectionContext context);
        object Create(Type abstractionType, IInjectionContext context, Type[] parameterTypes, object[] parameters);

        Type[] GetImplementationTypes(Type abstractionType);
    }
}