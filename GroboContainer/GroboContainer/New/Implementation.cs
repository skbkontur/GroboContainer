using System;
using System.Collections;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public class Implementation : IImplementation
    {
        private readonly object configurationLock = new object();
        private readonly Type implementationType;
        private volatile Hashtable factories;
        private volatile IClassFactory noArgumentsFactory;

        public Implementation(Type implementationType)
        {
            this.implementationType = implementationType;
        }

        #region IImplementation Members

        public Type ObjectType
        {
            get { return implementationType; }
        }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            IClassFactory classFactory = ChooseFactory(creationContext, parameterTypes);
            return classFactory;
        }

        #endregion

        private IClassFactory ChooseFactory(ICreationContext creationContext, Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0)
            {
                if (noArgumentsFactory == null)
                    lock (configurationLock)
                        if (noArgumentsFactory == null)
                            noArgumentsFactory = creationContext.BuildFactory(implementationType, Type.EmptyTypes);
                return noArgumentsFactory;
            }
            var types = new TypeArray(parameterTypes);
            IClassFactory factory;
            if ((factory = TryGetFactory(types)) == null)
                lock (configurationLock)
                    if ((factory = TryGetFactory(types)) == null)
                    {
                        if (factories == null)
                            factories = new Hashtable();
                        factories.Add(types, factory = creationContext.BuildFactory(implementationType, parameterTypes));
                    }
            return factory;
        }

        private IClassFactory TryGetFactory(TypeArray types)
        {
            if (factories == null) return null;
            var classFactory = (IClassFactory) factories[types];
            return classFactory;
        }
    }
}