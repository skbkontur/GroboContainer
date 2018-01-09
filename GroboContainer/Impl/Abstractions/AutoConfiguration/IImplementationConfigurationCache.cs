using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public interface IImplementationConfigurationCache
    {
        IImplementationConfiguration GetOrCreate(IImplementation implementation);
    }
}