using System;
using System.Reflection;

namespace GroboContainer.Impl.ClassCreation
{
    public interface IFuncHelper
    {
        MethodInfo GetBuildCreateFuncMethodInfo(Type funcType);
        MethodInfo GetBuildGetFuncMethodInfo(Type funcType);
        bool IsFunc(Type type);
    }
}