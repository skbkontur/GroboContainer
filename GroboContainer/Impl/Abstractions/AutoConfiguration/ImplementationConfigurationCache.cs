using System;
using System.Collections.Concurrent;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class ImplementationConfigurationCache : IImplementationConfigurationCache
    {
        private readonly ConcurrentDictionary<IImplementation, IImplementationConfiguration> map =
            new ConcurrentDictionary<IImplementation, IImplementationConfiguration>();

        private readonly Func<IImplementation, IImplementationConfiguration> valueFactory;

        public ImplementationConfigurationCache()
        {
            valueFactory = CreateImplementation;
        }

        #region IImplementationConfigurationCache Members

        public IImplementationConfiguration GetOrCreate(IImplementation implementation)
        {
            return map.GetOrAdd(implementation, valueFactory);
        }

        #endregion

        private static IImplementationConfiguration CreateImplementation(IImplementation implementation)
        {
            return new AutoImplementationConfiguration(implementation);
        }
    }
}