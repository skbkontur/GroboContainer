using System;
using System.Collections.Concurrent;

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
            return resolvedInstancesCache.AddOrUpdate(
                requestedType,
                t => factoryFunc(context.Container, t),
                (t, i) =>
                    {
                        context.Reused(requestedType);
                        return i;
                    });
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
        private readonly ConcurrentDictionary<Type, object> resolvedInstancesCache = new ConcurrentDictionary<Type, object>();
    }
}