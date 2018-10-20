using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.Contexts
{
    public class NoContextHolder : IContextHolder
    {
        public static readonly NoContextHolder Instance = new NoContextHolder();

        private NoContextHolder()
        {
        }

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