using System;

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
    public class Container : IContainer, IContainerForFuncBuilder
    {
        internal Container(IInternalContainer internalContainer, IContextHolder holder, ILog currentLog)
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
            :
                this(configuration, null)
        {
        }

        #region IContainer Members

        public IContainerConfigurator Configurator { get { return internalContainer.Configurator; } }

        public T Get<T>()
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Get<T>(context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Get(Type type)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Get(type, context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T[] GetAll<T>()
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.GetAll<T>(context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object[] GetAll(Type type)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.GetAll(type, context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T>()
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create<T>(context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T>(T1 arg1)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create<T1, T>(context, arg1);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T>(T1 arg1, T2 arg2)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T>(context, arg1, arg2);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T3, T>(T1 arg1, T2 arg2, T3 arg3)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T3, T>(context, arg1, arg2, arg3);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public T Create<T1, T2, T3, T4, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create<T1, T2, T3, T4, T>(context, arg1, arg2, arg3, arg4);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Create(Type abstractionType)
        {
            //return Create(abstractionType, Type.EmptyTypes, new object[0]);
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create(abstractionType, context);
            }
            catch(Exception e)
            {
                throw new ContainerException(context.GetLog().GetLog(), e);
            }
        }

        public object Create(Type abstractionType, Type[] parameterTypes, object[] parameters)
        {
            IInjectionContext context = GetContext();
            try
            {
                return internalContainer.Create(abstractionType, context, parameterTypes, parameters);
            }
            catch(Exception e)
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
                ILog log = lastConstructedLog;
                return (log != null ? log.GetLog() : "<no>");
            }
        }

        public void Dispose()
        {
            IInjectionContext context = holder.GetContext(internalContainer);
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

        public IContainer MakeChildContainer()
        {
            return new Container(internalContainer.MakeChild(), new NoContextHolder(), null);
        }

        public static IContainer CreateWithChilds(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator, IContainerSelector selector)
        {
            return new Container(new InternalContainer(new CompositeContainerContext(configuration, classWrapperCreator, selector)),
                                 new NoContextHolder(), null);
        }

        private IInjectionContext GetContext()
        {
            IInjectionContext context = holder.GetContext(internalContainer);
            lastConstructedLog = context.GetLog();
            return context;
        }

        private readonly IContextHolder holder;
        private readonly IInternalContainer internalContainer;
        private volatile ILog lastConstructedLog;
    }
}