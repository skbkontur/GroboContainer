using System;
using System.Collections;
using GroboContainer.Impl.Abstractions.AutoConfiguration;

namespace GroboContainer.Impl.Abstractions
{
    public class AbstractionConfigurationCollection : IAbstractionConfigurationCollection
    {
        private readonly Hashtable cache = new Hashtable();
        private readonly IAutoAbstractionConfigurationFactory factory;
        private readonly object lockObject = new object();

        public AbstractionConfigurationCollection(IAutoAbstractionConfigurationFactory factory)
        {
            this.factory = factory;
        }

        #region IAbstractionConfigurationCollection Members

        public IAbstractionConfiguration Get(Type abstractionType)
        {
            IAbstractionConfiguration result = Read(abstractionType);
            if (result == null)
                lock (lockObject)
                {
                    result = Read(abstractionType);
                    if (result == null)
                        cache.Add(abstractionType, result = factory.CreateByType(abstractionType));
                }
            return result;
        }

        public void Add(Type abstractionType,
                        IAbstractionConfiguration abstractionConfiguration)
        {
            lock (lockObject)
            {
                if (cache.ContainsKey(abstractionType))
                    throw new InvalidOperationException(string.Format("Тип {0} уже сконфигурирован", abstractionType));
                cache.Add(abstractionType, abstractionConfiguration);
            }
        }

        public IAbstractionConfiguration[] GetAll()
        {
            return cache.GetValues<IAbstractionConfiguration>();
        }

        #endregion

        private IAbstractionConfiguration Read(Type key)
        {
            return (IAbstractionConfiguration) cache[key];
        }
    }
}