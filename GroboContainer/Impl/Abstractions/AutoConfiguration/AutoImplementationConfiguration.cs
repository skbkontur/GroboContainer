using System;

using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class AutoImplementationConfiguration : IImplementationConfiguration
    {
        public AutoImplementationConfiguration(IImplementation implementation)
        {
            this.implementation = implementation;
        }

        private readonly object configurationLock = new object();
        private readonly IImplementation implementation;
        private volatile object instance;

        #region IImplementationConfiguration Members

        public Type ObjectType => implementation.ObjectType;

        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            if (instance == null)
                lock (configurationLock)
                    if (instance == null)
                    {
                        var noParametersFactory = implementation.GetFactory(Type.EmptyTypes, creationContext);
                        return instance = noParametersFactory.Create(context, EmptyArray<object>.Instance);
                    }
            context.Reused(ObjectType);
            return instance;
        }

        public void DisposeInstance()
        {
            var impl = instance;
            if (impl != null && impl is IDisposable)
                ((IDisposable)impl).Dispose();
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            var classFactory = implementation.GetFactory(parameterTypes, creationContext);
            return classFactory;
        }

        #endregion
    }
}