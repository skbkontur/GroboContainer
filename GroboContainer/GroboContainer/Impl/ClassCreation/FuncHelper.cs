using System;
using System.Collections.Generic;
using System.Reflection;

namespace GroboContainer.Impl.ClassCreation
{
    public class FuncHelper : IFuncHelper
    {
        private readonly MethodInfo buildGetFuncMethodInfo;
        private readonly IDictionary<int, MethodInfo> funcArgumentCountToMethodMap = new Dictionary<int, MethodInfo>();
        private readonly HashSet<Type> supportedFuncs = new HashSet<Type>();

        public FuncHelper()
        {
            Type type = typeof (IFuncBuilder);
            foreach (MethodInfo info in type.GetMethods())
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
                }
            }
            if (buildGetFuncMethodInfo == null)
                throw new MissingMethodException(type.ToString(), "BuildGetFunc");
        }

        #region IFuncHelper Members

        public MethodInfo GetBuildGetFuncMethodInfo(Type funcType)
        {
            Type[] genericArguments = funcType.GetGenericArguments();
            int length = genericArguments.Length;
            if (length != 1)
                throw new InvalidOperationException(
                    string.Format("Функции получения с {0} аргументами на поддерживаются",
                                  length - 1));
            return buildGetFuncMethodInfo.MakeGenericMethod(genericArguments);
        }

        public MethodInfo GetBuildCreateFuncMethodInfo(Type funcType)
        {
            MethodInfo result;
            Type[] genericArguments = funcType.GetGenericArguments();
            int length = genericArguments.Length;
            if (!funcArgumentCountToMethodMap.TryGetValue(length, out result))
                throw new InvalidOperationException(string.Format(
                                                        "Функции создания с {0} аргументами на поддерживаются",
                                                        length - 1));
            return result.MakeGenericMethod(genericArguments);
        }

        public bool IsFunc(Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition &&
                   supportedFuncs.Contains(type.GetGenericTypeDefinition());
        }

        #endregion
    }
}