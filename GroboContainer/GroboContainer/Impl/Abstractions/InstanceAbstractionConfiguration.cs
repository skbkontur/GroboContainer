using System;
using GroboContainer.Core;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class InstanceAbstractionConfiguration : IAbstractionConfiguration
    {
        private readonly IImplementationConfiguration[] implementationConfigurations;

        public InstanceAbstractionConfiguration(IClassWrapperCreator classWrapperCreator, Type abstractionType, object[] instances)
        {
            if (instances == null || instances.Length == 0)
                throw new ArgumentException("instances");
            implementationConfigurations = new IImplementationConfiguration[instances.Length];
            for (int i = 0; i < implementationConfigurations.Length; i++)
            {
                Type type = instances[i].GetType();
                if (!abstractionType.IsAssignableFrom(type))
                    throw new ArgumentException("Заданная реализация на являются объектами типа " + abstractionType +
                                                " (реальный тип " + type + ")");
                implementationConfigurations[i] = new InstanceImplementationConfiguration(classWrapperCreator, instances[i]);
            }
        }

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        #endregion
    }
}