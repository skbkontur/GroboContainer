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
            if (classWrapperCreator != null && ReferenceEquals(instance, classWrapperCreator.UnWrap(instance)))
                this.instance = classWrapperCreator.WrapAndCreate(instance);
            else
                this.instance = instance;
            instanceType = instance.GetType();
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