using System;
using System.Diagnostics;
using GroboContainer.Config;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl
{
    public class InternalContainer : IInternalContainer
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IFuncBuilder builder;
        private readonly IContainerConfigurator containerConfigurator;
        private readonly IContainerContext containerContext;
        private readonly ICreationContext creationContext;

        public InternalContainer(IContainerConfiguration configuration) : this(new ContainerContext(configuration))
        {
        }

        public InternalContainer(IContainerContext containerContext)
        {
            this.containerContext = containerContext;
            creationContext = containerContext.CreationContext;
            builder = containerContext.FuncBuilder;
            abstractionConfigurationCollection = containerContext.AbstractionConfigurationCollection;
            containerConfigurator = new ContainerConfigurator(containerContext.AbstractionConfigurationCollection);
        }

        #region IInternalContainer Members

        public IInternalContainer MakeChild()
        {
            return new InternalContainer(containerContext.MakeChildContext());
        }

        public Func<T> BuildCreateFunc<T>(IInjectionContext context)
        {
            return builder.BuildCreateFunc<T>(context);
        }

        public Func<T> BuildGetFunc<T>(IInjectionContext context)
        {
            return builder.BuildGetFunc<T>(context);
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
            return (T) Get(typeof (T), context);
        }

        public T Create<T>(IInjectionContext context)
        {
            return (T) Create(typeof (T), context);
        }

        public T Create<T1, T>(IInjectionContext context, T1 arg1)
        {
            return (T) CreateImpl(typeof (T), context, new[] {typeof (T1)}, arg1);
        }

        public T Create<T1, T2, T>(IInjectionContext context, T1 arg1, T2 arg2)
        {
            return (T) CreateImpl(typeof (T), context, new[] {typeof (T1), typeof (T2)}, arg1, arg2);
        }

        public T Create<T1, T2, T3, T>(IInjectionContext context, T1 arg1, T2 arg2, T3 arg3)
        {
            return (T) CreateImpl(typeof (T), context,
                                  new[] {typeof (T1), typeof (T2), typeof (T3)},
                                  arg1, arg2, arg3);
        }

        public T Create<T1, T2, T3, T4, T>(IInjectionContext context, T1 arg1, T2 arg2,
                                           T3 arg3, T4 arg4)
        {
            return (T) CreateImpl(typeof (T), context,
                                  new[] {typeof (T1), typeof (T2), typeof (T3), typeof (T4)},
                                  arg1, arg2, arg3, arg4);
        }

        public object Get(Type type, IInjectionContext context)
        {
            try
            {
                context.BeginGet(type);
                IImplementationConfiguration configuration = GetSingleImplementation(type);
                return configuration.GetOrCreateInstance(context, creationContext);
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
            Type type = typeof (T);
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
            return CreateImpl(abstractionType, context, Type.EmptyTypes, new object[0]);
        }


        public Type[] GetImplementationTypes(Type abstractionType)
        {
            IImplementationConfiguration[] all = GetImplementations(abstractionType);
            var result = new Type[all.Length];
            for (int i = 0; i < all.Length; i++)
                result[i] = all[i].ObjectType;
            return result;
        }

        public void CallDispose()
        {
            IAbstractionConfiguration[] configurations = abstractionConfigurationCollection.GetAll();
            foreach (IAbstractionConfiguration configuration in configurations)
                if (configuration != null) CallDisposeOnConfiguration(configuration);
        }


        public IContainerConfigurator Configurator
        {
            get { return containerConfigurator; }
        }

        public object Create(Type abstractionType, IInjectionContext context,
                             Type[] argumentTypes, object[] args)
        {
            return CreateImpl(abstractionType, context, argumentTypes, args);
        }

        #endregion

        private object CreateImpl(Type abstractionType, IInjectionContext context,
                                  Type[] argumentTypes, params object[] args)
        {
            try
            {
                context.BeginCreate(abstractionType);
                IImplementationConfiguration configuration = GetSingleImplementation(abstractionType);
                IClassFactory factory = configuration.GetFactory(argumentTypes, creationContext);
                return factory.Create(context, args);
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
            IImplementationConfiguration[] all = GetImplementations(abstractionType);
            if (all.Length == 0)
                throw new NoImplementationException(abstractionType);

            if (all.Length > 1)
                throw new ManyImplementationsException(abstractionType, all);
            return all[0];
        }

        private T[] GetInstances<T>(IInjectionContext context, Type abstractionType)
        {
            IImplementationConfiguration[] implementations = GetImplementations(abstractionType);
            var result = new T[implementations.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = (T) implementations[i].GetOrCreateInstance(context, creationContext);
            return result;
        }

        private static void CallDisposeOnConfiguration(IAbstractionConfiguration configuration)
        {
            Debug.WriteLine("CallDisposeOnConfiguration start");
            IImplementationConfiguration[] implementations = configuration.GetImplementations();
            foreach (IImplementationConfiguration implementation in implementations)
                implementation.DisposeInstance();
            Debug.WriteLine("CallDisposeOnConfiguration end");
        }

        private IImplementationConfiguration[] GetImplementations(Type type)
        {
            IImplementationConfiguration[] implementations =
                abstractionConfigurationCollection.Get(type).GetImplementations();
            return implementations;
        }
    }
}