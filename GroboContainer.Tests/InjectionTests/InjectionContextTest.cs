using System.Threading;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;
using GroboContainer.Tests.ImplTests;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.InjectionTests
{
    public class InjectionContextTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            internalContainer = NewMock<IInternalContainer>();
            log = NewMock<IGroboContainerLog>();
            holder = null;
            injectionContext = new InjectionContext(internalContainer, log, GetHolder);
        }

        private IContextHolder GetHolder(IInjectionContext context, int threadId)
        {
            Assert.AreSame(injectionContext, context);
            Assert.IsNull(holder, "duplicate call");
            holder = NewMock<IContextHolder>();
            return holder;
        }

        [Test]
        public void TestCycle()
        {
            var type = typeof(int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);

            log.ExpectBeginConstruct(typeof(long));
            injectionContext.BeginConstruct(typeof(long));
            log.ExpectGetLog("zzz");
            log.ExpectBeginConstruct(type);
            RunMethodWithException<CyclicDependencyException>(() => injectionContext.BeginConstruct(type), "zzz");
        }

        [Test]
        public void TestGetContainers()
        {
            Assert.IsNull(holder);
            var returnedContainer = injectionContext.Container;
            var contextHolder = holder;
            Assert.IsNotNull(contextHolder);
            Assert.That(returnedContainer, Is.InstanceOf<Container>());
            Assert.AreSame(returnedContainer, injectionContext.Container);
            Assert.AreSame(contextHolder, holder);
        }

        [Test]
        public void TestKillNoHolder()
        {
            var type = typeof(int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);
            log.ExpectEndConstruct(type);
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestKillWithContainer()
        {
            var type = typeof(int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);
            injectionContext.Container.ToString(); //container created
            log.ExpectEndConstruct(type);
            holder.ExpectKillContext();
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestLogging()
        {
            var type = typeof(int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);

            log.ExpectEndConstruct(type);
            injectionContext.EndConstruct(type);

            log.ExpectBeginCreate(type);
            injectionContext.BeginCreate(type);

            log.ExpectEndCreate(type);
            injectionContext.EndCreate(type);

            log.ExpectCrash();
            injectionContext.Crash();

            var type1 = typeof(long);
            log.ExpectBeginGet(type1);
            injectionContext.BeginGet(type1);

            log.ExpectEndGet(type1);
            injectionContext.EndGet(type1);

            log.ExpectBeginGetAll(type1);
            injectionContext.BeginGetAll(type1);

            log.ExpectEndGetAll(type1);
            injectionContext.EndGetAll(type1);

            log.ExpectReused(type1);
            injectionContext.Reused(type1);
        }

        [Test]
        public void TestRealConstructor()
        {
            var container = GetMock<IInternalContainer>();
            container.Expect(c => c.CreateNewLog()).Return(new GroboContainerLog("root"));
            injectionContext = new InjectionContext(container);
            Assert.That(injectionContext.Container, Is.InstanceOf<Container>());
            Assert.That(((ContextHolder)((IContainerInternals)injectionContext.Container).ContextHolder).OwnerThreadId, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
        }

        [Test]
        public void TestStupid()
        {
            Assert.AreSame(internalContainer, injectionContext.InternalContainer);
            Assert.AreSame(log, injectionContext.GetLog());
        }

        private IInternalContainer internalContainer;
        private IGroboContainerLog log;
        private InjectionContext injectionContext;
        private IContextHolder holder;
    }
}