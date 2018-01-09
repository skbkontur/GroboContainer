using System.Threading;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.ContextsTests
{
    public abstract class ContextTestBase : CoreTestBase
    {
        protected static void CheckGet(IContextHolder holder)
        {
            var container = GetMock<IInternalContainer>();
            container.Expect(c => c.CreateNewLog()).Return(null);
            var context = holder.GetContext(container);
            Assert.That(context, Is.InstanceOf<InjectionContext>());
            Assert.That(((InjectionContext)context).ThreadId, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
        }
    }
}