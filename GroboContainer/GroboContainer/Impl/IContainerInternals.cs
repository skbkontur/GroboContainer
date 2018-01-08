using GroboContainer.Impl.Contexts;

namespace GroboContainer.Impl
{
    public interface IContainerInternals
    {
        IContextHolder ContextHolder { get; }
    }
}