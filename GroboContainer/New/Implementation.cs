using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public class Implementation : IImplementation
    {
        public Implementation(Type implementationType)
        {
            ObjectType = implementationType;
        }

        private IClassFactory ChooseFactory(ICreationContext creationContext, Type[] parameterTypes)
        {
            if (parameterTypes.Length == 0)
            {
                if (noArgumentsFactory == null)
                {
                    lock (configurationLock)
                    {
                        if (noArgumentsFactory == null)
                            noArgumentsFactory = creationContext.BuildFactory(ObjectType, Type.EmptyTypes);
                    }
                }
                return noArgumentsFactory;
            }

            IClassFactory factory;
            if ((factory = TryGetFactory(parameterTypes)) == null)
            {
                lock (configurationLock)
                {
                    if ((factory = TryGetFactory(parameterTypes)) == null)
                    {
                        if (factories == null)
                            factories = new ConcurrentDictionary<Type[], IClassFactory>(TypeArrayEqualityComparer.Instance);
                        factories[parameterTypes] = factory = creationContext.BuildFactory(ObjectType, parameterTypes);
                    }
                }
            }
            return factory;
        }

        private IClassFactory TryGetFactory(Type[] types)
        {
            return factories != null && factories.TryGetValue(types, out var classFactory)
                       ? classFactory
                       : null;
        }

        public Type ObjectType { get; }

        public IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext)
        {
            var classFactory = ChooseFactory(creationContext, parameterTypes);
            return classFactory;
        }

        private readonly object configurationLock = new object();
        private volatile ConcurrentDictionary<Type[], IClassFactory> factories;
        private volatile IClassFactory noArgumentsFactory;

        internal sealed class TypeArrayEqualityComparer : IEqualityComparer<Type[]>
        {
            public bool Equals(Type[] x, Type[] y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode(Type[] x)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(x);
            }

            public static readonly TypeArrayEqualityComparer Instance = new TypeArrayEqualityComparer();
        }
    }
}