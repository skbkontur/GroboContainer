using System;

namespace GroboContainer.Impl.Interfaces
{
    public interface IInterfaceConfigurationCollection
    {
        IInterfaceConfiguration Get(Type interfaceType, string[] requireContracts);
        void Add(Type interfaceType, string[] requireContracts, IInterfaceConfiguration interfaceConfiguration);
    }
}