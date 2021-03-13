using System.Threading;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.InjectionTests
{
    public class InjectionContextTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            internalContainerMock = GetMock<IInternalContainer>();
            logMock = GetMock<IGroboContainerLog>();
            holderMock = null;
            injectionContext = new InjectionContext(internalContainerMock.Object, logMock.Object, GetHolder);
        }

        private IContextHolder GetHolder(IInjectionContext context, int threadId)
        {
            Assert.AreSame(injectionContext, context);
            Assert.IsNull(holderMock, "duplicate call");
            holderMock = GetMock<IContextHolder>();
            return holderMock.Object;
        }

        [Test]
        public void TestCycle()
        {
            var type = typeof(int);
            logMock.Setup(x => x.BeginConstruct(type));
            injectionContext.BeginConstruct(type);

            logMock.Setup(x => x.BeginConstruct(typeof(long)));
            injectionContext.BeginConstruct(typeof(long));
            logMock.Setup(x => x.GetLog()).Returns("zzz");
            logMock.Setup(x => x.BeginConstruct(type));
            RunMethodWithException<CyclicDependencyException>(() => injectionContext.BeginConstruct(type), "zzz");
        }

        [Test]
        public void TestGetContainers()
        {
            Assert.IsNull(holderMock);
            var returnedContainer = injectionContext.Container;
            Assert.IsNotNull(holderMock?.Object);
            Assert.That(returnedContainer, Is.InstanceOf<Container>());
            Assert.AreSame(returnedContainer, injectionContext.Container);
        }

        [Test]
        public void TestKillNoHolder()
        {
            var type = typeof(int);
            logMock.Setup(x => x.BeginConstruct(type));
            injectionContext.BeginConstruct(type);
            logMock.Setup(x => x.EndConstruct(type));
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestKillWithContainer()
        {
            var type = typeof(int);
            logMock.Setup(x => x.BeginConstruct(type));
            injectionContext.BeginConstruct(type);
            injectionContext.Container.ToString(); //container created
            logMock.Setup(x => x.EndConstruct(type));
            holderMock.Setup(x => x.KillContext());
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestLogging()
        {
            var type = typeof(int);
            logMock.Setup(x => x.BeginConstruct(type));
            injectionContext.BeginConstruct(type);

            logMock.Setup(x => x.EndConstruct(type));
            injectionContext.EndConstruct(type);

            logMock.Setup(x => x.BeginCreate(type));
            injectionContext.BeginCreate(type);

            logMock.Setup(x => x.EndCreate(type));
            injectionContext.EndCreate(type);

            logMock.Setup(x => x.Crash());
            injectionContext.Crash();

            var type1 = typeof(long);
            logMock.Setup(x => x.BeginGet(type1));
            injectionContext.BeginGet(type1);

            logMock.Setup(x => x.EndGet(type1));
            injectionContext.EndGet(type1);

            logMock.Setup(x => x.BeginGetAll(type1));
            injectionContext.BeginGetAll(type1);

            logMock.Setup(x => x.EndGetAll(type1));
            injectionContext.EndGetAll(type1);

            logMock.Setup(x => x.Reused(type1));
            injectionContext.Reused(type1);
        }

        [Test]
        public void TestRealConstructor()
        {
            var internalContainerMock2 = GetMock<IInternalContainer>();
            internalContainerMock2.Setup(c => c.CreateNewLog()).Returns(new GroboContainerLog("root"));
            injectionContext = new InjectionContext(internalContainerMock2.Object);
            Assert.That(injectionContext.Container, Is.InstanceOf<Container>());
            Assert.That(((ContextHolder)((IContainerInternals)injectionContext.Container).ContextHolder).OwnerThreadId, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
        }

        [Test]
        public void TestStupid()
        {
            Assert.AreSame(internalContainerMock.Object, injectionContext.InternalContainer);
            Assert.AreSame(logMock.Object, injectionContext.GetLog());
        }

        private Mock<IInternalContainer> internalContainerMock;
        private Mock<IGroboContainerLog> logMock;
        private InjectionContext injectionContext;
        private Mock<IContextHolder> holderMock;
    }
}