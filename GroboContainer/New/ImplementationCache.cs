using System;
using System.Collections.Concurrent;

namespace GroboContainer.New
{
    public class ImplementationCache : IImplementationCache
    {
        public ImplementationCache()
        {
            valueFactory = CreateImplementation;
        }

        public IImplementation GetOrCreate(Type implementationType)
        {
            return map.GetOrAdd(implementationType, valueFactory);
        }

        private static IImplementation CreateImplementation(Type type)
        {
            return new Implementation(type);
        }

        private readonly Func<Type, IImplementation> valueFactory;
        private readonly ConcurrentDictionary<Type, IImplementation> map = new ConcurrentDictionary<Type, IImplementation>();
    }
}