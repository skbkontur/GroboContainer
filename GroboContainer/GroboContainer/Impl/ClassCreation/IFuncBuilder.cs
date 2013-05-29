using System;
using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IFuncBuilder
    {
        Func<T> BuildGetFunc<T>(IInjectionContext context);
        Func<T> BuildCreateFunc<T>(IInjectionContext context);
        Func<T1, T> BuildCreateFunc<T1, T>(IInjectionContext context);
        Func<T1, T2, T> BuildCreateFunc<T1, T2, T>(IInjectionContext context);
        Func<T1, T2, T3, T> BuildCreateFunc<T1, T2, T3, T>(IInjectionContext context);
        Func<T1, T2, T3, T4, T> BuildCreateFunc<T1, T2, T3, T4, T>(IInjectionContext context);
    }
}