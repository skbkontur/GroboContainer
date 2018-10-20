using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

            IClassFactory factory;
            if ((factory = TryGetFactory(parameterTypes)) == null)
                lock (configurationLock)
                    if ((factory = TryGetFactory(parameterTypes)) == null)
                    {
                        if (factories == null)
                            factories = new ConcurrentDictionary<Type[], IClassFactory>(TypeArrayEqualityComparer.Instance);
                        factories[parameterTypes] = factory = creationContext.BuildFactory(ObjectType, parameterTypes);
                    }
            return factory;
        }

        private IClassFactory TryGetFactory(Type[] types)
        {
            return factories != null && factories.TryGetValue(types, out var classFactory)
                       ? classFactory
                       : null;
        }

        private readonly object configurationLock = new object();
        private volatile ConcurrentDictionary<Type[], IClassFactory> factories;
        private volatile IClassFactory noArgumentsFactory;

        #region IImplementation Members

        public Type ObjectType { get; }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            IClassFactory classFactory = ChooseFactory(creationContext, parameterTypes);
            return classFactory;
        }

        #endregion

        internal sealed class TypeArrayEqualityComparer : IEqualityComparer<Type[]>
        {
            public static readonly TypeArrayEqualityComparer Instance = new TypeArrayEqualityComparer();

            public bool Equals(Type[] x, Type[] y)
            {
                if (x == y)
                    return true;
                if (x == null)
                    return false;
                if (y == null)
                    return false;
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; i++)
                    if (x[i] == y[i])
                        return false;
                return true;
            }

            public int GetHashCode(Type[] x)
            {
                int result = 0;
                for(int i = 0; i < x.Length; ++i)
                    result = (result * 397) ^ x[i].GetHashCode();
                return result;
            }
        }
    }
}