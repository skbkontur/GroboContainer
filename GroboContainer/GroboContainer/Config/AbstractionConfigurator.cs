using System;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Config
{
    public class AbstractionConfigurator : IAbstractionConfigurator
    {
        private readonly IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private readonly Type abstractionType;

        public AbstractionConfigurator(Type abstractionType,
                                       IAbstractionConfigurationCollection abstractionConfigurationCollection)
        {
            this.abstractionType = abstractionType;
            this.abstractionConfigurationCollection = abstractionConfigurationCollection;
        }

        #region IAbstractionConfigurator Members

        public void UseInstances(params object[] instances)
        {
            abstractionConfigurationCollection.Add(abstractionType,
                                                   new InstanceAbstractionConfiguration(abstractionType, instances));
        }

        public void Fail()
        {
            abstractionConfigurationCollection.Add(abstractionType,
                                                   new StupidAbstractionConfiguration(
                                                       new ForbiddenImplementationConfiguration(abstractionType)));
        }

        #endregion
    }
}