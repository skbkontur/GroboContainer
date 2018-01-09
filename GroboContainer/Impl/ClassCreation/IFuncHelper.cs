using System;
using System.Reflection;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IFuncHelper
    {
        MethodInfo GetBuildCreateFuncMethodInfo(Type funcType);
        MethodInfo GetBuildGetFuncMethodInfo(Type funcType);
        MethodInfo GetBuildLazyMethodInfo(Type lazyType);
        bool IsLazy(Type type);
        bool IsFunc(Type type);
    }
}