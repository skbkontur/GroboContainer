using System;
using System.Collections.Concurrent;
using System.Linq;

using GroboContainer.Impl.Abstractions.AutoConfiguration;

namespace GroboContainer.Impl.Abstractions
{
    public class AbstractionConfigurationCollection : IAbstractionConfigurationCollection
    {
        public AbstractionConfigurationCollection(IAutoAbstractionConfigurationFactory factory)
        {
            createByType = factory.CreateByType;
        }

        public IAbstractionConfiguration Get(Type abstractionType)
        {
            if (abstractionType.IsGenericType && !cache.ContainsKey(abstractionType))
            {
                var genericTypeDefinition = abstractionType.GetGenericTypeDefinition();
                if (cache.TryGetValue(genericTypeDefinition, out var configuration))
                    return cache.GetOrAdd(abstractionType, configuration);
            }

            return cache.GetOrAdd(abstractionType, createByType);
        }

        public void Add(Type abstractionType, IAbstractionConfiguration abstractionConfiguration)
        {
            if (!cache.TryAddOrUpdate(abstractionType, abstractionConfiguration, c => c.GetImplementations().Length == 0))
                throw new InvalidOperationException($"Container is already configured for type {abstractionType}");
        }

        public IAbstractionConfiguration[] GetAll()
        {
            return cache.Values.ToArray();
        }

        private readonly Func<Type, IAbstractionConfiguration> createByType;
        private readonly ConcurrentDictionary<Type, IAbstractionConfiguration> cache = new ConcurrentDictionary<Type, IAbstractionConfiguration>();
    }
}