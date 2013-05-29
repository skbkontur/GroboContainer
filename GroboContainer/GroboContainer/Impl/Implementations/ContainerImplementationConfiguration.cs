using System;
using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public class ContainerImplementationConfiguration : IImplementationConfiguration
    {
        #region IImplementationConfiguration Members

        public Type ObjectType
        {
            get { return typeof (Container); }
        }

        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            context.Reused(typeof (IContainer));
            return context.Container;
        }

        //NOTE do nothing
        public void DisposeInstance()
        {
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}