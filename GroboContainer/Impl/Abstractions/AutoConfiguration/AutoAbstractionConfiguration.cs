using System;

using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class AutoAbstractionConfiguration : IAbstractionConfiguration
    {
        public AutoAbstractionConfiguration(Type abstractionType,
                                            IAbstractionsCollection abstractionsCollection,
                                            IImplementationConfigurationCache implementationConfigurationCache)
        {
            var abstraction = abstractionsCollection.Get(abstractionType);
            var implementations = abstraction.GetImplementations();
            implementationConfigurations = new IImplementationConfiguration[implementations.Length];
            for (var i = 0; i < implementations.Length; i++)
                implementationConfigurations[i] = implementationConfigurationCache.GetOrCreate(implementations[i]);
        }

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        #endregion

        private readonly IImplementationConfiguration[] implementationConfigurations;
    }
}