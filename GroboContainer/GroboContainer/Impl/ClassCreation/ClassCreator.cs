using System;
using System.Reflection;
using System.Reflection.Emit;

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

        #region IClassCreator Members

        public Func<IInternalContainer, IInjectionContext, object[], object> BuildConstructionDelegate(ContainerConstructorInfo constructorInfo, Type wrapperType)
        {
            return EmitConstruct(constructorInfo.ConstructorInfo, constructorInfo.ParametersInfo, wrapperType);
        }

        public IClassFactory BuildFactory(ContainerConstructorInfo constructorInfo, Type wrapperType)
        {
            return new ClassFactory(EmitConstruct(constructorInfo.ConstructorInfo, constructorInfo.ParametersInfo, wrapperType), constructorInfo.ConstructorInfo.ReflectedType);
        }

        #endregion

        private static MethodInfo GetMethod(string methodname, Type type, params Type[] types)
        {
            MethodInfo method = type.GetMethod(methodname, types);
            if(method == null)
                throw new MissingMethodException(type.ToString(), methodname);
            return method;
        }

        private Func<IInternalContainer, IInjectionContext, object[], object> EmitConstruct(ConstructorInfo constructorInfo, int[] parametersInfo, Type wrapperType)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object),
                                           new[]
                                               {
                                                   typeof(IInternalContainer), typeof(IInjectionContext),
                                                   typeof(object[])
                                               }, true);
            ILGenerator generator = method.GetILGenerator();

            for(int i = 0; i < constructorInfo.GetParameters().Length; i++)
            {
                ParameterInfo parameterInfo = constructorInfo.GetParameters()[i];
                ProcessParameter(parameterInfo, generator, parametersInfo != null ? parametersInfo[i] : -1);
            }
            generator.Emit(OpCodes.Newobj, constructorInfo);

            if(wrapperType != null)
            {
                var wrapperConstructor = wrapperType.GetConstructor(new[] {typeof(object)});
                if(wrapperConstructor == null)
                    throw new NoConstructorException(wrapperType);
                generator.Emit(OpCodes.Newobj, wrapperConstructor);
            }

            generator.Emit(OpCodes.Ret);
            var @delegate =
                (Func<IInternalContainer, IInjectionContext, object[], object>)
                method.CreateDelegate(typeof(Func<IInternalContainer, IInjectionContext, object[], object>));
            return @delegate;
        }

        private void ProcessParameter(ParameterInfo parameterInfo, ILGenerator generator, int parameterMarker)
        {
            Type parameterType = parameterInfo.ParameterType;
            if(parameterMarker != -1)
            {
                generator.Emit(OpCodes.Ldarg_2); //parameters
                generator.Emit(OpCodes.Ldc_I4, parameterMarker);
                generator.Emit(OpCodes.Ldelem_Ref);
                if(parameterType.IsValueType)
                {
                    if(!IsNullable(parameterType))
                        EmitCrashIfValueIsNull(generator);
                    generator.Emit(OpCodes.Unbox_Any, parameterType);
                }
                else
                    generator.Emit(OpCodes.Castclass, parameterType);
            }
            else
            {
                generator.Emit(OpCodes.Ldarg_0); //container -> this for methods
                generator.Emit(OpCodes.Ldarg_1); //arg0: context
                if(parameterType.IsArray)
                    ProcessArray(generator, parameterType);
                else
                    ProcessNonArray(parameterInfo, generator, parameterType);
            }
        }

        private static void EmitCrashIfValueIsNull(ILGenerator generator)
        {
            Label box = generator.DefineLabel();
            generator.Emit(OpCodes.Dup);
            generator.Emit(OpCodes.Brtrue, box);
            ConstructorInfo crashConstructor = typeof(ArgumentException).GetConstructor(new[] {typeof(string)});
            generator.Emit(OpCodes.Ldstr, "bad parameter");
            generator.Emit(OpCodes.Newobj, crashConstructor);
            generator.Emit(OpCodes.Throw);
            generator.MarkLabel(box);
        }

        private static bool IsNullable(Type parameterType)
        {
            return (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        //private static void PushRequiredContracts(ILGenerator generator, string[] requireContracts)
        //{
        //    generator.Emit(OpCodes.Ldc_I4, requireContracts.Length);
        //    generator.Emit(OpCodes.Newarr, typeof (string)); //contracts
        //    for (int i = 0; i < requireContracts.Length; ++i)
        //    {
        //        generator.Emit(OpCodes.Dup);
        //        generator.Emit(OpCodes.Ldc_I4, i);
        //        generator.Emit(OpCodes.Ldstr, requireContracts[i]);
        //        generator.Emit(OpCodes.Stelem_Ref);
        //    }
        //}

        private void ProcessNonArray(ParameterInfo parameterInfo, ILGenerator generator, Type parameterType)
        {
            if(funcHelper.IsFunc(parameterType))
                ProcessFuncParameter(parameterInfo, generator, parameterType);
            else
                generator.Emit(OpCodes.Callvirt, getMethod.MakeGenericMethod(parameterType));
        }

        private void ProcessArray(ILGenerator generator, Type parameterType)
        {
            generator.Emit(OpCodes.Callvirt,
                           getAllMethod.MakeGenericMethod(parameterType.GetElementType()));
        }

        private void ProcessFuncParameter(ParameterInfo parameterInfo, ILGenerator generator, Type parameterType)
        {
            if(parameterInfo.Name.StartsWith("get", StringComparison.InvariantCultureIgnoreCase))
                generator.Emit(OpCodes.Callvirt, funcHelper.GetBuildGetFuncMethodInfo(parameterType));
            else if(parameterInfo.Name.StartsWith("create", StringComparison.InvariantCultureIgnoreCase))
            {
                generator.Emit(OpCodes.Callvirt,
                               funcHelper.GetBuildCreateFuncMethodInfo(parameterInfo.ParameterType));
            }
            else
            {
                throw new NotSupportedException(
                    string.Format(
                        "Невозможно разрешить параметр '{0}' типа {1}. Должен иметь имя с префиксом 'get' или 'create'",
                        parameterInfo.Name, parameterType));
            }
        }

        private static readonly Type internalContainerType = typeof(IInternalContainer);
        private readonly IFuncHelper funcHelper;
        private readonly MethodInfo getAllMethod;
        private readonly MethodInfo getMethod;
    }
}