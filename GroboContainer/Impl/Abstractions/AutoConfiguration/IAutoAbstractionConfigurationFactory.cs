using System;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public interface IAutoAbstractionConfigurationFactory
    {
        IAbstractionConfiguration CreateByType(Type abstractionType);
    }
}