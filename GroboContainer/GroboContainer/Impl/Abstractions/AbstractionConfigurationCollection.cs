using System;
using System.Collections.Concurrent;
using System.Linq;
using GroboContainer.Impl.Abstractions.AutoConfiguration;

namespace GroboContainer.Impl.Abstractions
{
    public class AbstractionConfigurationCollection : IAbstractionConfigurationCollection
    {
        private readonly ConcurrentDictionary<Type, IAbstractionConfiguration> cache = new ConcurrentDictionary<Type, IAbstractionConfiguration>();
        private readonly Func<Type, IAbstractionConfiguration> createByType;

        public AbstractionConfigurationCollection(IAutoAbstractionConfigurationFactory factory)
        {
            createByType = factory.CreateByType;
        }

        #region IAbstractionConfigurationCollection Members

        public IAbstractionConfiguration Get(Type abstractionType)
        {
            return cache.GetOrAdd(abstractionType, createByType);
        }

        public void Add(Type abstractionType, IAbstractionConfiguration abstractionConfiguration)
        {
            if (!cache.TryAddOrUpdate(abstractionType, abstractionConfiguration, c => c.GetImplementations().Length == 0))
                throw new InvalidOperationException(string.Format("Тип {0} уже сконфигурирован", abstractionType));
        }

        public IAbstractionConfiguration[] GetAll()
        {
            return cache.Values.ToArray();
        }

        #endregion
    }
}