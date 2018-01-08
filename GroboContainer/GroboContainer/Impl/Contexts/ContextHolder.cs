using System.Threading;
using GroboContainer.Impl.Injection;

namespace GroboContainer.Impl.Contexts
{
    public class ContextHolder : IContextHolder
    {
        private volatile IInjectionContext currentContext;

        public ContextHolder(IInjectionContext currentContext, int ownerThreadId)
        {
            this.currentContext = currentContext;
            OwnerThreadId = ownerThreadId;
        }

        public int OwnerThreadId { get; }

        #region IContextHolder Members

        public IInjectionContext GetContext(IInternalContainer worker)
        {
            if (currentContext == null || Thread.CurrentThread.ManagedThreadId != OwnerThreadId)
                return new InjectionContext(worker);
            return currentContext;
        }

        public void KillContext()
        {
            currentContext = null;
        }

        #endregion
    }
}