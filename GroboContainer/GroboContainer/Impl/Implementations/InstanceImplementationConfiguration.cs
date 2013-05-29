using System;
using System.Diagnostics;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public class InstanceImplementationConfiguration : IImplementationConfiguration
    {
        private readonly object instance;

        public InstanceImplementationConfiguration(object instance)
        {
            this.instance = instance;
        }

        #region IImplementationConfiguration Members

        public Type ObjectType
        {
            get { return instance.GetType(); }
        }


        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            context.Reused(instance.GetType());
            return instance;
        }

        public void DisposeInstance()
        {
            if (instance is IDisposable)
            {
                Debug.WriteLine(instance.GetType().FullName);
                ((IDisposable) instance).Dispose();
            }
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}