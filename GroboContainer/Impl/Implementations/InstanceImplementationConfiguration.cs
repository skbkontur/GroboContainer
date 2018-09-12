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
        public InstanceImplementationConfiguration(IClassWrapperCreator classWrapperCreator, object instance)
        {
            if (classWrapperCreator != null && ReferenceEquals(instance, classWrapperCreator.UnWrap(instance)))
                this.instance = classWrapperCreator.WrapAndCreate(instance);
            else
                this.instance = instance;
            ObjectType = instance.GetType();
        }

        private readonly object instance;

        #region IImplementationConfiguration Members

        public Type ObjectType { get; }

        public object GetOrCreateInstance(IInjectionContext context, ICreationContext creationContext)
        {
            context.Reused(ObjectType);
            return instance;
        }

        public void DisposeInstance()
        {
            if (instance is IDisposable)
            {
                Debug.WriteLine(ObjectType.FullName);
                ((IDisposable)instance).Dispose();
            }
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}