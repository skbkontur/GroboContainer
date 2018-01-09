using System;
using GroboContainer.Config.Generic;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.New;

namespace GroboContainer.Config
{
    public class ContainerConfigurator : IContainerConfigurator
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IClassWrapperCreator classWrapperCreator;
	    private readonly IImplementationConfigurationCache implementationConfigurationCache;
	    private readonly IImplementationCache implementationCache;

	    public ContainerConfigurator(IAbstractionConfigurationCollection abstractionConfigurationCollection,
		    IClassWrapperCreator classWrapperCreator,
		    IImplementationConfigurationCache implementationConfigurationCache, IImplementationCache implementationCache)
	    {
		    this.abstractionConfigurationCollection = abstractionConfigurationCollection;
		    this.classWrapperCreator = classWrapperCreator;
		    this.implementationConfigurationCache = implementationConfigurationCache;
		    this.implementationCache = implementationCache;
	    }

        #region IContainerConfigurator Members

        public IAbstractionConfigurator<T> ForAbstraction<T>()
        {
	        return new AbstractionConfigurator<T>(abstractionConfigurationCollection, classWrapperCreator,
		        implementationConfigurationCache, implementationCache);
        }

        public IAbstractionConfigurator ForAbstraction(Type anstractionType)
        {
	        return new AbstractionConfigurator(anstractionType, abstractionConfigurationCollection, classWrapperCreator,
		        implementationConfigurationCache, implementationCache);
        }

        #endregion
    }
}