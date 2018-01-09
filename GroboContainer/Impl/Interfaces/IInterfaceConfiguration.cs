using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Interfaces
{
    public interface IInterfaceConfiguration
    {
        IImplementationConfiguration[] GetImplementations();
    }
}