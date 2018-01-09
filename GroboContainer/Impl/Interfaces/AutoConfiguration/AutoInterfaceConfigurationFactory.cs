using System;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Interfaces.AutoConfiguration
{
    public class AutoInterfaceConfigurationFactory : IAutoInterfaceConfigurationFactory
    {
        private readonly IImplementationTypesCollection implementationTypesCollection;
        private readonly IImplementationConfigurationCollection implementationConfigurationCollection;

        public AutoInterfaceConfigurationFactory(IImplementationTypesCollection implementationTypesCollection,
                                                 IImplementationConfigurationCollection
                                                     implementationConfigurationCollection)
        {
            this.implementationTypesCollection = implementationTypesCollection;
            this.implementationConfigurationCollection = implementationConfigurationCollection;
        }
        public IInterfaceConfiguration CreateByType(Type abstractionType, string[] requireContracts)
        {
            return new InterfaceConfiguration(abstractionType, requireContracts, implementationTypesCollection, implementationConfigurationCollection);
        }
    }
}