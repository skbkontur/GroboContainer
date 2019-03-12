using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;

using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.ClassCreation
{
    //TODO слишком жирный класс
    public class ClassCreator : IClassCreator
    {
        public ClassCreator(IFuncHelper funcHelper)
        {
            this.funcHelper = funcHelper;
            getMethod = GetMethod("Get", internalContainerType, typeof(IInjectionContext));
            getAllMethod = GetMethod("GetAll", internalContainerType, typeof(IInjectionContext));
        }

        private static MethodInfo GetMethod(string methodName, Type type, params Type[] types)
        {
            var method = type.GetMethod(methodName, types);
            if (method == null)
                throw new MissingMethodException(type.ToString(), methodName);
            return method;
        }

        private Func<IInternalContainer, IInjectionContext, object[], object> EmitConstruct(ConstructorInfo constructorInfo, int[] parametersInfo, Type wrapperType)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), TypeArray<IInternalContainer, IInjectionContext, object[]>.Instance, typeof(ClassCreator), true);
            using (var il = new GroboIL(method))
            {
                var constructorParameters = constructorInfo.GetParameters();
                for (var i = 0; i < constructorParameters.Length; i++)
                    ProcessParameter(constructorParameters[i], il, parametersInfo != null ? parametersInfo[i] : -1);
                il.Newobj(constructorInfo);

                if (wrapperType != null)
                {
                    var wrapperConstructor = wrapperType.GetConstructor(TypeArray<object>.Instance);
                    if (wrapperConstructor == null)
                        throw new NoConstructorException(wrapperType);
                    il.Newobj(wrapperConstructor);
                }

                il.Ret();
            }
            var @delegate =
                (Func<IInternalContainer, IInjectionContext, object[], object>)
                method.CreateDelegate(typeof(Func<IInternalContainer, IInjectionContext, object[], object>));
            return @delegate;
        }

        private void ProcessParameter(ParameterInfo parameterInfo, GroboIL il, int parameterMarker)
        {
            var parameterType = parameterInfo.ParameterType;
            if (parameterMarker != -1)
            {
                il.Ldarg(2); //parameters
                il.Ldc_I4(parameterMarker);
                il.Ldelem(typeof(object));
                if (parameterType.IsValueType)
                {
                    if (!IsNullable(parameterType))
                        EmitCrashIfValueIsNull(il);
                    il.Unbox_Any(parameterType);
                }
                else
                    il.Castclass(parameterType);
            }
            else
            {
                il.Ldarg(0); //container -> this for methods
                il.Ldarg(1); //arg0: context
                if (parameterType.IsArray)
                    ProcessArray(il, parameterType.GetElementType());
                else if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ProcessArray(il, parameterType.GetGenericArguments()[0]);
                else
                    ProcessNonArray(parameterInfo, il, parameterType);
            }
        }

        private static void EmitCrashIfValueIsNull(GroboIL il)
        {
            var box = il.DefineLabel("box");
            il.Dup();
            il.Brtrue(box);
            var crashConstructor = typeof(ArgumentException).GetConstructor(TypeArray<string>.Instance);
            il.Ldstr("bad parameter");
            il.Newobj(crashConstructor);
            il.Throw();
            il.MarkLabel(box);
        }

        private static bool IsNullable(Type parameterType)
        {
            return parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private void ProcessNonArray(ParameterInfo parameterInfo, GroboIL il, Type parameterType)
        {
            if (funcHelper.IsLazy(parameterType))
                ProcessLazyParameter(il, parameterType);
            else if (funcHelper.IsFunc(parameterType))
                ProcessFuncParameter(parameterInfo, il, parameterType);
            else
                il.Call(getMethod.MakeGenericMethod(parameterType));
        }

        private void ProcessArray(GroboIL il, Type elementType)
        {
            il.Call(getAllMethod.MakeGenericMethod(elementType));
        }

        private void ProcessLazyParameter(GroboIL il, Type parameterType)
        {
            il.Call(funcHelper.GetBuildLazyMethodInfo(parameterType));
        }

        private void ProcessFuncParameter(ParameterInfo parameterInfo, GroboIL il, Type parameterType)
        {
            if (parameterInfo.Name.StartsWith("get", StringComparison.InvariantCultureIgnoreCase))
                il.Call(funcHelper.GetBuildGetFuncMethodInfo(parameterType));
            else if (parameterInfo.Name.StartsWith("create", StringComparison.InvariantCultureIgnoreCase))
                il.Call(funcHelper.GetBuildCreateFuncMethodInfo(parameterInfo.ParameterType));
            else
                throw new NotSupportedException($"Failed to resolve func parameter {parameterInfo.Name} of type {parameterType}. Func parameter names should have 'get' or 'create' prefix.");
        }

        public IClassFactory BuildFactory(ContainerConstructorInfo constructorInfo, Type wrapperType)
        {
            return new ClassFactory(EmitConstruct(constructorInfo.ConstructorInfo, constructorInfo.ParametersInfo, wrapperType), constructorInfo.ConstructorInfo.ReflectedType);
        }

        private static readonly Type internalContainerType = typeof(IInternalContainer);
        private readonly IFuncHelper funcHelper;
        private readonly MethodInfo getAllMethod;
        private readonly MethodInfo getMethod;
    }
}