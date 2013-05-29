using System;
using GroboContainer.Config.Generic;
using GroboContainer.Impl.Abstractions;

namespace GroboContainer.Config
{
    public class ContainerConfigurator : IContainerConfigurator
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;

        public ContainerConfigurator(IAbstractionConfigurationCollection abstractionConfigurationCollection)
        {
            this.abstractionConfigurationCollection = abstractionConfigurationCollection;
        }

        #region IContainerConfigurator Members

        public IAbstractionConfigurator<T> ForAbstraction<T>()
        {
            return new AbstractionConfigurator<T>(abstractionConfigurationCollection);
        }

        public IAbstractionConfigurator ForAbstraction(Type anstractionType)
        {
            return new AbstractionConfigurator(anstractionType, abstractionConfigurationCollection);
        }

        #endregion
    }
}