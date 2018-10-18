using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GroboContainer.Config;
using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;

namespace GroboContainer.Core
{
    public class Container : IContainer, IContainerForFuncBuilder, IContainerInternals
    {
        public Container(IInternalContainer internalContainer, IContextHolder holder, IGroboContainerLog currentLog)
        {
            this.internalContainer = internalContainer;
            this.holder = holder;
            lastConstructedLog = currentLog;
        }

        public Container(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator)
            : this(new InternalContainer(configuration, classWrapperCreator), new NoContextHolder(), null)
        {
        }

        public Container(IContainerConfiguration configuration)
            : this(configuration, null)
        {
        }

        IContextHolder IContainerInternals.ContextHolder => holder;

        public IContainer MakeChildContainer()
        {
            return new Container(internalContainer.MakeChild(), new NoContextHolder(), null);
        }

        public static IContainer CreateWithChilds(IContainerConfiguration configuration,
                                                  IClassWrapperCreator classWrapperCreator, IContainerSelector selector)
        {
            return
                new Container(
                    new InternalContainer(new CompositeContainerContext(configuration, classWrapperCreator, selector)),
                    new NoContextHolder(), null);
        }

        private IInjectionContext GetContext()
        {
            var context = holder.GetContext(internalContainer);
            lastConstructedLog = context.GetLog();
            return context;
        }

        private readonly IContextHolder holder;
        private readonly IInternalContainer internalContainer;
        private volatile IGroboContainerLog lastConstructedLog;
        private static readonly MethodInfo getLazyFuncMethod = typeof(Container).GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == "GetLazyFunc" && x.IsGenericMethod);
        private static readonly Type getLazyFuncMethodReturnType = getLazyFuncMethod.ReturnType.GetGenericTypeDefinition();
        private static readonly IDictionary<Type, MethodInfo> getCreationFuncMethods = typeof(Container).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == "GetCreationFunc" && x.IsGenericMethod).ToDictionary(x => x.ReturnType.GetGenericTypeDefinition());

        #region IContainer Members

        public string Name => internalContainer.Name;

        public IContainerConfigurator Configurator => internalContainer.Configurator;

        public T Get<T>()
        {
            var context = GetContext();
            try
            {
                return internalContainer.Get<T>(context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Get(Type type)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Get(type, context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T[] GetAll<T>()
        {
            var context = GetContext();
            try
            {
                return internalContainer.GetAll<T>(context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object[] GetAll(Type type)
        {
            var context = GetContext();
            try
            {
                return internalContainer.GetAll(type, context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T>()
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create<T>(context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T>(T1 arg1)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create<T1, T>(context, arg1);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T>(T1 arg1, T2 arg2)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T>(context, arg1, arg2);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T3, T>(context, arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T3, T4, T>(context, arg1, arg2, arg3, arg4);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Create(Type abstractionType)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create(abstractionType, context);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Create(Type abstractionType, Type[] parameterTypes, object[] parameters)
        {
            var context = GetContext();
            try
            {
                return internalContainer.Create(abstractionType, context, parameterTypes, parameters);
            }
            catch (Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public Type[] GetImplementationTypes(Type abstractionType)
        {
            return internalContainer.GetImplementationTypes(abstractionType);
        }

        public string LastConstructionLog
        {
            get
            {
                var log = lastConstructedLog;
                return (log != null ? log.GetLog() : "<no>");
            }
        }

        public Func<T> GetLazyFunc<T>()
        {
            return Get<T>;
        }

        public Func<T> GetCreationFunc<T>()
        {
            return Create<T>;
        }

        public Func<T1, T> GetCreationFunc<T1, T>()
        {
            return Create<T1, T>;
        }

        public Func<T1, T2, T> GetCreationFunc<T1, T2, T>()
        {
            return Create<T1, T2, T>;
        }

        public Func<T1, T2, T3, T> GetCreationFunc<T1, T2, T3, T>()
        {
            return Create<T1, T2, T3, T>;
        }

        public Func<T1, T2, T3, T4, T> GetCreationFunc<T1, T2, T3, T4, T>()
        {
            return Create<T1, T2, T3, T4, T>;
        }

        public Delegate GetLazyFunc(Type funcType)
        {
            return internalContainer.GetLazyFunc(funcType, type =>
                {
                    if (!type.IsGenericType || type.GetGenericTypeDefinition() != getLazyFuncMethodReturnType)
                        throw new InvalidOperationException(string.Format("Тип {0} не поддерживаются в качестве функции получения", type));
                    return (Delegate)getLazyFuncMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(this, Arrays<object>.Empty);
                });
        }

        public Delegate GetCreationFunc(Type funcType)
        {
            return internalContainer.GetCreationFunc(funcType, type =>
                {
                    if (!type.IsGenericType || !getCreationFuncMethods.TryGetValue(type.GetGenericTypeDefinition(), out var methodInfo))
                        throw new InvalidOperationException(string.Format("Тип {0} не поддерживаются в качестве функции создания", type));
                    return (Delegate)methodInfo.MakeGenericMethod(type.GetGenericArguments()).Invoke(this, Arrays<object>.Empty);
                });
        }

        public void Dispose()
        {
            var context = holder.GetContext(internalContainer);
            context.InternalContainer.CallDispose();
        }

        #endregion

        #region IContainerForFuncBuilder Members

        public T CreateForFunc<T>()
        {
            return internalContainer.Create<T>(GetContext());
        }

        public T CreateForFunc<T1, T>(T1 arg1)
        {
            return internalContainer.Create<T1, T>(GetContext(), arg1);
        }

        public T CreateForFunc<T1, T2, T>(T1 arg1, T2 arg2)
        {
            return internalContainer.Create<T1, T2, T>(GetContext(), arg1, arg2);
        }

        public T CreateForFunc<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3)
        {
            return internalContainer.Create<T1, T2, T3, T>(GetContext(), arg1, arg2, arg3);
        }

        public T CreateForFunc<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return internalContainer.Create<T1, T2, T3, T4, T>(GetContext(), arg1, arg2, arg3, arg4);
        }

        public T GetForFunc<T>()
        {
            return internalContainer.Get<T>(GetContext());
        }

        #endregion
    }
}