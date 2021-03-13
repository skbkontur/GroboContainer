using System;
using System.Threading;
using System.Threading.Tasks;

using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;

using NUnit.Framework;

namespace GroboContainer.Tests.ContextsTests
{
    public class ContextHolderTest : ContextTestBase
    {
        [Test]
        public void TestAnotherThread()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var internalContainerMock = GetMock<IInternalContainer>();
            internalContainerMock.Setup(c => c.CreateNewLog()).Returns((IGroboContainerLog)null);
            IContextHolder holder = new ContextHolder(new InjectionContext(internalContainerMock.Object), threadId);
            var task = Task.Run(() =>
                {
                    Assert.AreNotEqual(threadId, Thread.CurrentThread.ManagedThreadId);
                    CheckGet(holder);
                });
            task.Wait(TimeSpan.FromSeconds(1));
            holder.KillContext();
            CheckGet(holder);
        }

        [Test]
        public void TestCurrentThread()
        {
            var internalContainerMock = GetMock<IInternalContainer>();
            internalContainerMock.Setup(c => c.CreateNewLog()).Returns((IGroboContainerLog)null);
            var context = new InjectionContext(internalContainerMock.Object);
            IContextHolder holder = new ContextHolder(context, Thread.CurrentThread.ManagedThreadId);

            var internalContainerMock2 = GetMock<IInternalContainer>();
            Assert.AreSame(context, holder.GetContext(internalContainerMock2.Object));
            holder.KillContext();
            CheckGet(holder);
        }
    }
}