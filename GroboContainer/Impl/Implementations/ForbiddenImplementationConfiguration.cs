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

        private readonly Type abstractionType;

        #region IImplementationConfiguration Members

        public Type ObjectType { get { throw new ForbiddenAbstractionException(abstractionType); } }

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

        #endregion
    }
}