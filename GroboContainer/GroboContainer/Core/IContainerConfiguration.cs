using GroboContainer.New;

namespace GroboContainer.Core
{
    public interface IContainerConfiguration : ITypeSource
    {
        string ContainerName { get; }
        ContainerMode Mode { get; }
    }
}