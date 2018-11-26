using System;
using System.Collections.Concurrent;

using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class ImplementationConfigurationCache : IImplementationConfigurationCache
    {
        public ImplementationConfigurationCache()
        {
            valueFactory = CreateImplementation;
        }

        public IImplementationConfiguration GetOrCreate(IImplementation implementation)
        {
            return map.GetOrAdd(implementation, valueFactory);
        }

        private static IImplementationConfiguration CreateImplementation(IImplementation implementation)
        {
            return new AutoImplementationConfiguration(implementation);
        }

        private readonly Func<IImplementation, IImplementationConfiguration> valueFactory;
        private readonly ConcurrentDictionary<IImplementation, IImplementationConfiguration> map = new ConcurrentDictionary<IImplementation, IImplementationConfiguration>();
    }
}