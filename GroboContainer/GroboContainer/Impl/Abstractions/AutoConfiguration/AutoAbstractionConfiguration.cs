using System;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class AutoAbstractionConfiguration : IAbstractionConfiguration
    {
        private readonly IImplementationConfiguration[] implementationConfigurations;

        public AutoAbstractionConfiguration(Type abstractionType,
                                            IAbstractionsCollection abstractionsCollection,
                                            IImplementationConfigurationCache implementationConfigurationCache)
        {
            IAbstraction abstraction = abstractionsCollection.Get(abstractionType);
            IImplementation[] implementations = abstraction.GetImplementations();
            implementationConfigurations = new IImplementationConfiguration[implementations.Length];
            for (int i = 0; i < implementations.Length; i++)
                implementationConfigurations[i] = implementationConfigurationCache.GetOrCreate(implementations[i]);
        }

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        #endregion
    }
}