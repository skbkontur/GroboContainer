using System;
using System.Collections.Concurrent;
using System.Linq;

using GroboContainer.Config;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;
using GroboContainer.New;

namespace GroboContainer.Impl
{
    public class InternalContainer : IInternalContainer
    {
        public InternalContainer(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator)
            : this(new ContainerContext(configuration, classWrapperCreator))
        {
        }

        public InternalContainer(IContainerContext containerContext)
        {
            this.containerContext = containerContext;
            creationContext = containerContext.CreationContext;
            builder = containerContext.FuncBuilder;
            abstractionConfigurationCollection = containerContext.AbstractionConfigurationCollection;
            Configurator = new ContainerConfigurator(containerContext.AbstractionConfigurationCollection,
                                                     containerContext.ClassWrapperCreator,
                                                     containerContext.ImplementationConfigurationCache,
                                                     containerContext.ImplementationCache);
        }

        private object CreateImpl(Type abstractionType, IInjectionContext context, Type[] argumentTypes, params object[] args)
        {
            try
            {
                context.BeginCreate(abstractionType);
                var configuration = GetSingleImplementation(abstractionType);
                var factory = configuration.GetFactory(argumentTypes, creationContext);
                return UnWrap(abstractionType, factory.Create(context, args));
            }
            catch (Exception)
            {
                context.Crash();
                throw;
            }
            finally
            {
                context.EndCreate(abstractionType);
            }
        }

        private IImplementationConfiguration GetSingleImplementation(Type abstractionType)
        {
            var all = GetImplementations(abstractionType);
            if (all.Length == 0)
                throw new NoImplementationException(abstractionType);

            if (all.Length > 1)
                throw new ManyImplementationsException(abstractionType, all);
            return all[0];
        }

        private T[] GetInstances<T>(IInjectionContext context, Type abstractionType)
        {
            var implementations = GetImplementations(abstractionType);
            var result = new T[implementations.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = (T)UnWrap(abstractionType, implementations[i].GetOrCreateInstance(context, creationContext));
            return result;
        }

        private IImplementationConfiguration[] GetImplementations(Type type)
        {
            var implementations =
                abstractionConfigurationCollection.Get(type).GetImplementations();
            return implementations;
        }

        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IFuncBuilder builder;
        private readonly IContainerContext containerContext;
        private readonly ICreationContext creationContext;
        private readonly ConcurrentDictionary<Type, Delegate> getLazyDelegates = new ConcurrentDictionary<Type, Delegate>();
        private readonly ConcurrentDictionary<Type, Delegate> getCreationDelegates = new ConcurrentDictionary<Type, Delegate>();

        #region IInternalContainer Members

        public IGroboContainerLog CreateNewLog()
        {
            var containerName = containerContext.Configuration.ContainerName;
            if ((containerContext.Configuration.Mode & ContainerMode.UseShortLog) == ContainerMode.UseShortLog)
                return new ShortGroboContainerLog(containerName);
            return new GroboContainerLog(containerName);
        }

        public string Name => containerContext.Configuration.ContainerName;

        public IInternalContainer MakeChild()
        {
            return new InternalContainer(containerContext.MakeChildContext());
        }

        public Lazy<T> BuildLazy<T>(IInjectionContext context)
        {
            return builder.BuildLazy<T>(context);
        }

        public Func<T> BuildGetFunc<T>(IInjectionContext context)
        {
            return builder.BuildGetFunc<T>(context);
        }

        public Func<T> BuildCreateFunc<T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T>(context);
        }

        public Func<T1, T> BuildCreateFunc<T1, T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T1, T>(context);
        }

        public Func<T1, T2, T> BuildCreateFunc<T1, T2, T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T1, T2, T>(context);
        }

        public Func<T1, T2, T3, T> BuildCreateFunc<T1, T2, T3, T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T1, T2, T3, T>(context);
        }

        public Func<T1, T2, T3, T4, T> BuildCreateFunc<T1, T2, T3, T4, T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T1, T2, T3, T4, T>(context);
        }

        public T Get<T>(IInjectionContext context)
        {
            return (T)Get(typeof(T), context);
        }

        public T Create<T>(IInjectionContext context)
        {
            return (T)Create(typeof(T), context);
        }

        public T Create<T1, T>(IInjectionContext context, T1 arg1)
        {
            return (T)CreateImpl(typeof(T), context, TypeArray<T1>.Instance, arg1);
        }

        public T Create<T1, T2, T>(IInjectionContext context, T1 arg1, T2 arg2)
        {
            return (T)CreateImpl(typeof(T), context, TypeArray<T1, T2>.Instance, arg1, arg2);
        }

        public T Create<T1, T2, T3, T>(IInjectionContext context, T1 arg1, T2 arg2, T3 arg3)
        {
            return (T)CreateImpl(typeof(T), context, TypeArray<T1, T2, T3>.Instance, arg1, arg2, arg3);
        }

        public T Create<T1, T2, T3, T4, T>(IInjectionContext context, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (T)CreateImpl(typeof(T), context, TypeArray<T1, T2, T3, T4>.Instance, arg1, arg2, arg3, arg4);
        }

        public object Get(Type type, IInjectionContext context)
        {
            try
            {
                context.BeginGet(type);
                var configuration = GetSingleImplementation(type);
                return UnWrap(type, configuration.GetOrCreateInstance(context, creationContext));
            }
            catch (Exception)
            {
                context.Crash();
                throw;
            }
            finally
            {
                context.EndGet(type);
            }
        }

        public T[] GetAll<T>(IInjectionContext context)
        {
            var type = typeof(T);
            try
            {
                context.BeginGetAll(type);
                return GetInstances<T>(context, type);
            }
            catch (Exception)
            {
                context.Crash();
                throw;
            }
            finally
            {
                context.EndGetAll(type);
            }
        }

        public object[] GetAll(Type type, IInjectionContext context)
        {
            try
            {
                context.BeginGetAll(type);
                return GetInstances<object>(context, type);
            }
            catch (Exception)
            {
                context.Crash();
                throw;
            }
            finally
            {
                context.EndGetAll(type);
            }
        }

        public object Create(Type abstractionType, IInjectionContext context)
        {
            return CreateImpl(abstractionType, context, Type.EmptyTypes, EmptyArray<object>.Instance);
        }

        public Type[] GetImplementationTypes(Type abstractionType)
        {
            var all = GetImplementations(abstractionType);
            var result = new Type[all.Length];
            for (var i = 0; i < all.Length; i++)
                result[i] = all[i].ObjectType;
            return result;
        }

        public void CallDispose()
        {
            var configurations = abstractionConfigurationCollection
                                 .GetAll()
                                 .Where(x => x != null)
                                 .SelectMany(x => x.GetImplementations())
                                 .OrderByDescending(x => x.InstanceCreationOrder);

            foreach (var configuration in configurations)
                configuration.DisposeInstance();
        }

        public IContainerConfigurator Configurator { get; }

        public object Create(Type abstractionType, IInjectionContext context, Type[] argumentTypes, object[] args)
        {
            return CreateImpl(abstractionType, context, argumentTypes, args);
        }

        private object UnWrap(Type abstractionType, object instance)
        {
            if (abstractionType.IsInterface || containerContext.ClassWrapperCreator == null)
                return instance;
            return containerContext.ClassWrapperCreator.UnWrap(instance);
        }

        public Delegate GetLazyFunc(Type funcType, Func<Type, Delegate> factory)
        {
            return getLazyDelegates.GetOrAdd(funcType, factory);
        }

        public Delegate GetCreationFunc(Type funcType, Func<Type, Delegate> factory)
        {
            return getCreationDelegates.GetOrAdd(funcType, factory);
        }

        #endregion
    }
}