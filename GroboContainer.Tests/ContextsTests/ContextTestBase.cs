using System.Threading;

using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;

using NUnit.Framework;

namespace GroboContainer.Tests.ContextsTests
{
    public abstract class ContextTestBase : CoreTestBase
    {
        protected void CheckGet(IContextHolder holder)
        {
            var internalContainerMock = GetMock<IInternalContainer>();
            internalContainerMock.Setup(c => c.CreateNewLog()).Returns((IGroboContainerLog)null);
            var context = holder.GetContext(internalContainerMock.Object);
            Assert.That(context, Is.InstanceOf<InjectionContext>());
            Assert.That(((InjectionContext)context).ThreadId, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
        }
    }
}