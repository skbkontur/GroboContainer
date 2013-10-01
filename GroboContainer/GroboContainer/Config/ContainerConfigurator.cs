using System;
using GroboContainer.Config.Generic;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;

namespace GroboContainer.Config
{
    public class ContainerConfigurator : IContainerConfigurator
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IClassWrapperCreator classWrapperCreator;

        public ContainerConfigurator(IAbstractionConfigurationCollection abstractionConfigurationCollection, IClassWrapperCreator classWrapperCreator)
        {
            this.abstractionConfigurationCollection = abstractionConfigurationCollection;
            this.classWrapperCreator = classWrapperCreator;
        }

        #region IContainerConfigurator Members

        public IAbstractionConfigurator<T> ForAbstraction<T>()
        {
            return new AbstractionConfigurator<T>(abstractionConfigurationCollection, classWrapperCreator);
        }

        public IAbstractionConfigurator ForAbstraction(Type anstractionType)
        {
            return new AbstractionConfigurator(anstractionType, abstractionConfigurationCollection, classWrapperCreator);
        }

        #endregion
    }
}