using System;

namespace GroboContainer.Impl.Abstractions
{
    public interface IAbstractionConfigurationCollection
    {
        IAbstractionConfiguration Get(Type abstractionType);
        void Add(Type abstractionType, IAbstractionConfiguration abstractionConfiguration);
        IAbstractionConfiguration[] GetAll();
    }
}