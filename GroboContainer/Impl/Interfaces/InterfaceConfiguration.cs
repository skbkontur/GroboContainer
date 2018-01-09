using System;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Interfaces
{
    public class InterfaceConfiguration : IInterfaceConfiguration
    {
        private readonly IImplementationConfiguration[] implementationConfigurations;

        public InterfaceConfiguration(Type interfaceType, string[] requireContracts,
                                      IImplementationTypesCollection implementationTypesCollection,
                                      IImplementationConfigurationCollection implementationConfigurationCollection)
        {
            Type[] types = implementationTypesCollection.GetImplementationTypes(interfaceType, requireContracts);
            implementationConfigurations = new IImplementationConfiguration[types.Length];
            for (int index = 0; index < types.Length; index++)
                implementationConfigurations[index] = implementationConfigurationCollection.Get(types[index]);
        }

        #region IInterfaceConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        #endregion
    }
}