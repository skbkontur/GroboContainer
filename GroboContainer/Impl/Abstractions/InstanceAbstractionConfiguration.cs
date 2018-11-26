using System;

using GroboContainer.Core;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class InstanceAbstractionConfiguration : IAbstractionConfiguration
    {
        public InstanceAbstractionConfiguration(IClassWrapperCreator classWrapperCreator, Type abstractionType, object[] instances)
        {
            if (instances == null || instances.Length == 0)
                throw new ArgumentException("instances");
            implementationConfigurations = new IImplementationConfiguration[instances.Length];
            for (var i = 0; i < implementationConfigurations.Length; i++)
            {
                var type = instances[i].GetType();
                if (!abstractionType.IsAssignableFrom(type))
                    throw new ArgumentException($"Instances of type {type} cannot be used as abstraction of type {abstractionType}");
                implementationConfigurations[i] = new InstanceImplementationConfiguration(classWrapperCreator, instances[i]);
            }
        }

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        private readonly IImplementationConfiguration[] implementationConfigurations;
    }
}