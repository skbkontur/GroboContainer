using System;
using System.Collections.Generic;

using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public class FactoryImplementationConfiguration : IImplementationConfiguration
    {
        public FactoryImplementationConfiguration(Type objectType, Func<IContainer, Type, object> factoryFunc)
        {
            ObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            this.factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        public Type ObjectType { get; }

        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext, Type requestedType)
        {
            if (!resolvedInstancesCache.ContainsKey(requestedType))
            {
                lock (configurationLock)
                {
                    if (!resolvedInstancesCache.ContainsKey(requestedType))
                    {
                        return resolvedInstancesCache[requestedType] = CreateRequestedTypeInstance(context, requestedType);
                    }
                }
            }
            context.Reused(requestedType);
            return resolvedInstancesCache[requestedType];
        }

        private object CreateRequestedTypeInstance(IInjectionContext context, Type requestedType)
        {
            context.BeginConstruct(requestedType);
            var instance = factoryFunc(context.Container, requestedType);
            context.EndConstruct(requestedType);
            return instance;
        }

        public void DisposeInstance()
        {
            foreach (var kvp in resolvedInstancesCache)
                if (kvp.Value is IDisposable disposable)
                    disposable.Dispose();
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new NotSupportedException();
        }

        private readonly Func<IContainer, Type, object> factoryFunc;
        private readonly object configurationLock = new object();
        private readonly Dictionary<Type, object> resolvedInstancesCache = new Dictionary<Type, object>();
    }
}