using System;
using System.Threading;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using NUnit.Framework;

namespace Tests.ContextsTests
{
    public class ContextHolderTest : ContextTestBase
    {
        [Test]
        public void TestAnotherThread()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            IContextHolder holder = new ContextHolder(new InjectionContext(NewMock<IInternalContainer>()), threadId);
            Action check = () =>
                               {
                                   Assert.AreNotEqual(threadId, Thread.CurrentThread.ManagedThreadId);
                                   CheckGet(holder);
                               };
            IAsyncResult result = check.BeginInvoke(null, null);
            Assert.That(result.AsyncWaitHandle.WaitOne(1000), "поток завис");
            holder.KillContext();
            CheckGet(holder);
        }

        [Test]
        public void TestCurrentThread()
        {
            var context = new InjectionContext(NewMock<IInternalContainer>());
            IContextHolder holder = new ContextHolder(context, Thread.CurrentThread.ManagedThreadId);

            Assert.AreSame(context, holder.GetContext(NewMock<IInternalContainer>()));
            holder.KillContext();
            CheckGet(holder);
        }
    }
}