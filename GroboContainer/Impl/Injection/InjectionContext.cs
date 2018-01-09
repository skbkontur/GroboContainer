using System;
using System.Collections.Generic;
using System.Threading;
using GroboContainer.Core;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Logging;

namespace GroboContainer.Impl.Injection
{
    public class InjectionContext : IInjectionContext
    {
        private readonly HashSet<Type> constructed = new HashSet<Type>();
        private readonly Func<IInjectionContext, IContextHolder> createHolder;
        private readonly object lockObject = new object();
        private readonly ILog log;
        private volatile Container container;
        private volatile IContextHolder contextHolder;

        internal InjectionContext(IInternalContainer internalContainer, ILog log,
                                  Func<IInjectionContext, IContextHolder> createHolder)
        {
            InternalContainer = internalContainer;
            this.log = log;
            this.createHolder = createHolder;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public InjectionContext(IInternalContainer internalContainer)
            : this(internalContainer, internalContainer.CreateNewLog(), null)
        {
            createHolder = CreateHolder;
        }

        public int ThreadId { get; }

        #region IInjectionContext Members

        public IContainerForFuncBuilder ContainerForFunc => GetContainer();

        public void BeginConstruct(Type classType)
        {
            log.BeginConstruct(classType);
            Begin(classType);
        }

        public void EndConstruct(Type classType)
        {
            log.EndConstruct(classType);
            End(classType);
        }

        public void Reused(Type classType)
        {
            log.Reused(classType);
        }

        public void Crash()
        {
            log.Crash();
        }

        public void BeginGet(Type type)
        {
            log.BeginGet(type);
        }

        public void EndGet(Type type)
        {
            log.EndGet(type);
        }

        public void BeginGetAll(Type type)
        {
            log.BeginGetAll(type);
        }

        public void EndGetAll(Type type)
        {
            log.EndGetAll(type);
        }

        public void BeginCreate(Type abstractionType)
        {
            log.BeginCreate(abstractionType);
        }

        public void EndCreate(Type abstractionType)
        {
            log.EndCreate(abstractionType);
        }

        public ILog GetLog()
        {
            return log;
        }

        public IContainer Container => GetContainer();

        public IInternalContainer InternalContainer { get; }

        #endregion

        private Container GetContainer()
        {
            //TODO MT bug
            if (container == null)
            {
                contextHolder = createHolder(this);
                container = new Container(InternalContainer, contextHolder, log);
            }
            return container;
        }

        private IContextHolder CreateHolder(IInjectionContext context)
        {
            return new ContextHolder(context, ThreadId);
        }

        private void Begin(Type type)
        {
            lock (lockObject)
                if (!constructed.Add(type))
                    throw new CyclicDependencyException(log.GetLog());
        }

        private void End(Type type)
        {
            lock (lockObject)
            {
                constructed.Remove(type);
                if (constructed.Count == 0 && contextHolder != null)
                    contextHolder.KillContext();
            }
        }
    }
}