using System;

namespace GroboContainer.Impl.Implementations
{
    public interface IImplementationConfigurationCollection
    {
        IImplementationConfiguration Get(Type implementationType);
    }
}