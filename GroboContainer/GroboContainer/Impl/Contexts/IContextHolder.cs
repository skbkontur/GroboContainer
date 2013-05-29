using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.Contexts
{
    public interface IContextHolder
    {
        IInjectionContext GetContext(IInternalContainer worker);
        void KillContext();
    }
}