using System;

namespace GroboContainer.Impl.Interfaces.AutoConfiguration
{
    public interface IAutoInterfaceConfigurationFactory
    {
        IInterfaceConfiguration CreateByType(Type abstractionType, string[] requireContracts);
    }
}