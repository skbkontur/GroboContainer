using System;
using System.Collections.Generic;
using System.Reflection;

namespace GroboContainer.Impl.ClassCreation
{
    public class FuncHelper : IFuncHelper
    {
        public FuncHelper()
        {
            var type = typeof(IFuncBuilder);
            foreach (var info in type.GetMethods())
            {
                switch (info.Name)
                {
                case "BuildCreateFunc":
                    funcArgumentCountToMethodMap.Add(info.GetGenericArguments().Length, info);
                    supportedFuncs.Add(info.ReturnType.GetGenericTypeDefinition());
                    break;
                case "BuildGetFunc":
                    if (buildGetFuncMethodInfo != null)
                        throw new InvalidOperationException("Duplicate method 'BuildGetFunc'");
                    buildGetFuncMethodInfo = info;
                    break;
                case "BuildLazy":
                    if (buildLazyMethodInfo != null)
                        throw new InvalidOperationException("Duplicate method 'BuildLazy'");
                    buildLazyMethodInfo = info;
                    break;
                }
            }
            if (buildLazyMethodInfo == null)
                throw new MissingMethodException(type.ToString(), "BuildLazy");
            if (buildGetFuncMethodInfo == null)
                throw new MissingMethodException(type.ToString(), "BuildGetFunc");
        }

        private readonly MethodInfo buildLazyMethodInfo;
        private readonly MethodInfo buildGetFuncMethodInfo;
        private readonly IDictionary<int, MethodInfo> funcArgumentCountToMethodMap = new Dictionary<int, MethodInfo>();
        private readonly HashSet<Type> supportedFuncs = new HashSet<Type>();

        #region IFuncHelper Members

        public MethodInfo GetBuildLazyMethodInfo(Type lazyType)
        {
            var genericArguments = lazyType.GetGenericArguments();
            if (genericArguments.Length != 1)
                throw new InvalidOperationException($"Invalid lazyType: {lazyType.FullName}");
            return buildLazyMethodInfo.MakeGenericMethod(genericArguments);
        }

        public MethodInfo GetBuildGetFuncMethodInfo(Type funcType)
        {
            var genericArguments = funcType.GetGenericArguments();
            var length = genericArguments.Length;
            if (length != 1)
                throw new InvalidOperationException(
                    $"Функции получения с {length - 1} аргументами на поддерживаются");
            return buildGetFuncMethodInfo.MakeGenericMethod(genericArguments);
        }

        public MethodInfo GetBuildCreateFuncMethodInfo(Type funcType)
        {
            MethodInfo result;
            var genericArguments = funcType.GetGenericArguments();
            var length = genericArguments.Length;
            if (!funcArgumentCountToMethodMap.TryGetValue(length, out result))
                throw new InvalidOperationException($"Функции создания с {length - 1} аргументами на поддерживаются");
            return result.MakeGenericMethod(genericArguments);
        }

        public bool IsLazy(Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        public bool IsFunc(Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition &&
                   supportedFuncs.Contains(type.GetGenericTypeDefinition());
        }

        #endregion
    }
}