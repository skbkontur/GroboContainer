using System;

using GroboContainer.Core;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class FactoryAbstractionConfiguration : IAbstractionConfiguration
    {
        public FactoryAbstractionConfiguration(Type objectType, Func<IContainer, Type, object> factoryFunc)
        {
            implementationConfigurations = new IImplementationConfiguration[]
                {
                    new FactoryImplementationConfiguration(objectType, factoryFunc)
                };
        }

        public IImplementationConfiguration[] GetImplementations() => implementationConfigurations;

        private readonly IImplementationConfiguration[] implementationConfigurations;
    }
}