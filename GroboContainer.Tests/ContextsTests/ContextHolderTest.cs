using System;
using System.Threading;

using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.ContextsTests
{
    public class ContextHolderTest : ContextTestBase
    {
        [Test]
        public void TestAnotherThread()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            IInternalContainer ic = GetMock<IInternalContainer>();
            ic.Expect(c => c.CreateNewLog()).Return(null);
            IContextHolder holder = new ContextHolder(new InjectionContext(ic), threadId);
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
            IInternalContainer ic = GetMock<IInternalContainer>();
            ic.Expect(c => c.CreateNewLog()).Return(null);
            var context = new InjectionContext(ic);
            IContextHolder holder = new ContextHolder(context, Thread.CurrentThread.ManagedThreadId);

            IInternalContainer ic2 = GetMock<IInternalContainer>();
            Assert.AreSame(context, holder.GetContext(ic2));
            holder.KillContext();
            CheckGet(holder);
        }
    }
}