using System;
using System.Linq;

using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Config
{
    public class AbstractionConfigurator : IAbstractionConfigurator
    {
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

        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly IClassWrapperCreator classWrapperCreator;
        private readonly IImplementationConfigurationCache implementationConfigurationCache;
        private readonly IImplementationCache implementationCache;
        private readonly Type abstractionType;

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
            var abstractionConfiguration = new StupidAbstractionConfiguration(implementationConfiguration);
            abstractionConfigurationCollection.Add(abstractionType, abstractionConfiguration);
        }

        public void UseTypes(Type[] types)
        {
            var implementationConfigurations = types
                .Select(x => implementationCache.GetOrCreate(x))
                .Select(x => implementationConfigurationCache.GetOrCreate(x))
                .ToArray();
            var abstractionConfiguration = new StupidAbstractionConfiguration(implementationConfigurations);
            abstractionConfigurationCollection.Add(abstractionType, abstractionConfiguration);
        }

        public void UseFactory(Func<IContainer, Type, object> factoryFunc)
        {
            var abstractionConfiguration = new FactoryAbstractionConfiguration(abstractionType, factoryFunc);
            abstractionConfigurationCollection.Add(abstractionType, abstractionConfiguration);
        }

        #endregion
    }
}