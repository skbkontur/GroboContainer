using System;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public class ForbiddenImplementationConfiguration : IImplementationConfiguration
    {
        public ForbiddenImplementationConfiguration(Type abstractionType)
        {
            this.abstractionType = abstractionType;
        }

        public Type ObjectType => throw new ForbiddenAbstractionException(abstractionType);

        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            throw new ForbiddenAbstractionException(abstractionType);
        }

        public void DisposeInstance()
        {
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new ForbiddenAbstractionException(abstractionType);
        }

        private readonly Type abstractionType;
    }
}