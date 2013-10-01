using System;
using System.Diagnostics;
using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

namespace GroboContainer.Impl.Implementations
{
    public class InstanceImplementationConfiguration : IImplementationConfiguration
    {
        private readonly object instance;
        private readonly Type instanceType;

        public InstanceImplementationConfiguration(IClassWrapperCreator classWrapperCreator, object instance)
        {
            instanceType = instance.GetType();
            this.instance = classWrapperCreator == null ? instance : classWrapperCreator.WrapAndCreate(instance);
        }

        #region IImplementationConfiguration Members

        public Type ObjectType
        {
            get { return instanceType; }
        }


        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            context.Reused(instanceType);
            return instance;
        }

        public void DisposeInstance()
        {
            if (instance is IDisposable)
            {
                Debug.WriteLine(instanceType.FullName);
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