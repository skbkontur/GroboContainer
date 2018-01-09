using System;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Config
{
    public class AbstractionConfigurator : IAbstractionConfigurator
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IClassWrapperCreator classWrapperCreator;
	    private readonly IImplementationConfigurationCache implementationConfigurationCache;
	    private readonly IImplementationCache implementationCache;
	    private readonly Type abstractionType;

	    public AbstractionConfigurator(Type abstractionType,
		    IAbstractionConfigurationCollection abstractionConfigurationCollection,
		    IClassWrapperCreator classWrapperCreator, IImplementationConfigurationCache implementationConfigurationCache,
		    IImplementationCache implementationCache)
	    {
		    this.abstractionType = abstractionType;
		    this.abstractionConfigurationCollection = abstractionConfigurationCollection;
		    this.classWrapperCreator = classWrapperCreator;
		    this.implementationConfigurationCache = implementationConfigurationCache;
		    this.implementationCache = implementationCache;
	    }

        #region IAbstractionConfigurator Members

        public void UseInstances(params object[] instances)
        {
            abstractionConfigurationCollection.Add(abstractionType,
                                                   new InstanceAbstractionConfiguration(classWrapperCreator, abstractionType, instances));
        }

        public void Fail()
        {
            abstractionConfigurationCollection.Add(abstractionType,
                                                   new StupidAbstractionConfiguration(
                                                       new ForbiddenImplementationConfiguration(abstractionType)));
        }

        public void UseType(Type type)
        {
			var implementation = implementationCache.GetOrCreate(type);
			var implementationConfiguration = implementationConfigurationCache.GetOrCreate(implementation);
			var abstractionConfiguration = new StupidAbstractionConfiguration(new[] { implementationConfiguration });
			abstractionConfigurationCollection.Add(abstractionType, abstractionConfiguration);
        }

        #endregion
    }
}