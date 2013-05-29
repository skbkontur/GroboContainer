using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.Contexts
{
    public class NoContextHolder : IContextHolder
    {
        #region IContextHolder Members

        public IInjectionContext GetContext(IInternalContainer worker)
        {
            return new InjectionContext(worker);
        }

        public void KillContext()
        {
        }

        #endregion
    }
}