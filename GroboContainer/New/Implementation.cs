using System;
using System.Collections.Concurrent;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public class Implementation : IImplementation
    {
        public Implementation(Type implementationType)
        {
            this.ObjectType = implementationType;
        }

        private IClassFactory ChooseFactory(ICreationContext creationContext, Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0)
            {
                if (noArgumentsFactory == null)
                    lock (configurationLock)
                        if (noArgumentsFactory == null)
                            noArgumentsFactory = creationContext.BuildFactory(ObjectType, Type.EmptyTypes);
                return noArgumentsFactory;
            }
            var types = new TypeArray(parameterTypes);
            IClassFactory factory;
            if ((factory = TryGetFactory(types)) == null)
                lock (configurationLock)
                    if ((factory = TryGetFactory(types)) == null)
                    {
                        if (factories == null)
                            factories = new ConcurrentDictionary<TypeArray, IClassFactory>();
                        factories[types] = factory = creationContext.BuildFactory(ObjectType, parameterTypes);
                    }
            return factory;
        }

        private IClassFactory TryGetFactory(TypeArray types)
        {
            return factories != null && factories.TryGetValue(types, out var classFactory)
                       ? classFactory
                       : null;
        }

        private readonly object configurationLock = new object();
        private volatile ConcurrentDictionary<TypeArray, IClassFactory> factories;
        private volatile IClassFactory noArgumentsFactory;

        #region IImplementation Members

        public Type ObjectType { get; }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            IClassFactory classFactory = ChooseFactory(creationContext, parameterTypes);
            return classFactory;
        }

        #endregion
    }
}