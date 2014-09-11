using System;
using System.Collections.Generic;
using System.Linq;
using GroboContainer.Impl.Abstractions.AutoConfiguration;

namespace GroboContainer.Impl.Abstractions
{
    public class AbstractionConfigurationCollection : IAbstractionConfigurationCollection
    {
        private readonly IDictionary<Type, IAbstractionConfiguration> cache = new Dictionary<Type, IAbstractionConfiguration>();
        private readonly object lockObject = new object();

        private readonly Func<Type, IAbstractionConfiguration> createByType;

        public AbstractionConfigurationCollection(IAutoAbstractionConfigurationFactory factory)
        {
            createByType = factory.CreateByType;
        }

        #region IAbstractionConfigurationCollection Members

        public IAbstractionConfiguration Get(Type abstractionType)
        {
            IAbstractionConfiguration result;

            if (!cache.TryGetValue(abstractionType, out result))
            {
                lock (lockObject)
                {
                    if (!cache.TryGetValue(abstractionType, out result))
                    {
                        cache.Add(abstractionType, result = createByType(abstractionType));
                    }
                }
            }

            return result;
        }

        public void Add(Type abstractionType, IAbstractionConfiguration abstractionConfiguration)
        {
            lock (lockObject)
            {
                IAbstractionConfiguration existedConfiguration;
                if (cache.TryGetValue(abstractionType, out existedConfiguration) && existedConfiguration.GetImplementations().Length != 0)
                    throw new InvalidOperationException(string.Format("Тип {0} уже сконфигурирован", abstractionType));

                cache[abstractionType] = abstractionConfiguration;
            }
        }

        public IAbstractionConfiguration[] GetAll()
        {
            return cache.Values.ToArray();
        }

        #endregion
    }
}