using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public interface IAbstractionConfiguration
    {
        IImplementationConfiguration[] GetImplementations();
    }
}