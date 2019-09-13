using System;

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
            return factoryFunc(context.Container, requestedType);
        }

        public void DisposeInstance()
        {
            // no instance is managed by container
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new NotSupportedException();
        }

        private readonly Func<IContainer, Type, object> factoryFunc;
    }
}