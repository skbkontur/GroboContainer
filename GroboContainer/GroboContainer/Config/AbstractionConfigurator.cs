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
        private readonly Type abstractionType;

        public AbstractionConfigurator(Type abstractionType, IAbstractionConfigurationCollection abstractionConfigurationCollection, IClassWrapperCreator classWrapperCreator)
        {
            this.abstractionType = abstractionType;
            this.abstractionConfigurationCollection = abstractionConfigurationCollection;
            this.classWrapperCreator = classWrapperCreator;
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
            abstractionConfigurationCollection.Add(abstractionType,
                                                   new StupidAbstractionConfiguration(new[]
                                                       {new AutoImplementationConfiguration(new Implementation(type))}));
        }

        #endregion
    }
}