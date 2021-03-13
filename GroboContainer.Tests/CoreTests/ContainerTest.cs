using System;
using System.Linq;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;

using Moq;

using NUnit.Framework;

using MockException = GroboContainer.Tests.ImplTests.MockException;

namespace GroboContainer.Tests.CoreTests
{
    public class ContainerTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            internalContainerMock = GetMock<IInternalContainer>();
            holderMock = GetMock<IContextHolder>();
            contextMock = GetMock<IInjectionContext>();
            logMock = GetMock<IGroboContainerLog>();
            container = new Container(internalContainerMock.Object, holderMock.Object, logMock.Object);
        }

        [Test]
        public void TestCrashGet()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            var mockException = new MockException();
            internalContainerMock.Setup(x => x.Get<int>(contextMock.Object)).Throws(mockException);
            logMock.Setup(x => x.GetLog()).Returns("zzz");
            RunMethodWithException<ContainerException>(() => container.Get<int>(),
                                                       exception =>
                                                           {
                                                               Assert.AreEqual("zzz", exception.Message);
                                                               Assert.AreSame(mockException, exception.InnerException);
                                                           });
        }

        [Test]
        public void TestCrashObjectGet()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            var mockException = new MockException();
            internalContainerMock.Setup(x => x.Get(typeof(int), contextMock.Object)).Throws(mockException);
            logMock.Setup(x => x.GetLog()).Returns("zzz");
            RunMethodWithException<ContainerException>(() => container.Get(typeof(int)),
                                                       exception =>
                                                           {
                                                               Assert.AreEqual("zzz", exception.Message);
                                                               Assert.AreSame(mockException, exception.InnerException);
                                                           });
        }

        [Test]
        public void TestCreateGeneric()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            internalContainerMock.Setup(x => x.Create<int>(contextMock.Object)).Returns(1);
            Assert.AreEqual(1, container.Create<int>());
            logMock.Setup(x => x.GetLog()).Returns("zzz");
            Assert.AreEqual("zzz", container.LastConstructionLog);
        }

        [Test]
        public void TestCreateGeneric2Args()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            var arg2 = Guid.NewGuid();
            internalContainerMock.Setup(x => x.Create<string, Guid, int>(contextMock.Object, "s", arg2)).Returns(1);
            Assert.AreEqual(1, container.Create<string, Guid, int>("s", arg2));
        }

        [Test]
        public void TestCreateGeneric3Args()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            var arg2 = Guid.NewGuid();
            internalContainerMock.Setup(x => x.Create<string, Guid, bool, int>(contextMock.Object, "s", arg2, true)).Returns(1);
            Assert.AreEqual(1, container.Create<string, Guid, bool, int>("s", arg2, true));
        }

        [Test]
        public void TestCreateGeneric4Args()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            var arg2 = Guid.NewGuid();
            var ints = new[] {2};
            internalContainerMock.Setup(x => x.Create<string, Guid, bool, int[], int>(contextMock.Object, "s", arg2, true, ints)).Returns(1);
            Assert.AreEqual(1, container.Create<string, Guid, bool, int[], int>("s", arg2, true, ints));
        }

        [Test]
        public void TestCreateGenericOneArg()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            internalContainerMock.Setup(x => x.Create<string, int>(contextMock.Object, "s")).Returns(1);
            Assert.AreEqual(1, container.Create<string, int>("s"));
        }

        [Test]
        public void TestDispose()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.InternalContainer).Returns(internalContainerMock.Object);
            internalContainerMock.Setup(x => x.CallDispose());
            container.Dispose();
        }

        [Test]
        public void TestNoLog()
        {
            container = new Container(new TestConfiguration(Enumerable.Empty<Type>()));
            Assert.AreEqual("<no>", container.LastConstructionLog);
        }

        [Test]
        public void TestStupid()
        {
            holderMock.Setup(x => x.GetContext(internalContainerMock.Object)).Returns(contextMock.Object);
            contextMock.Setup(x => x.GetLog()).Returns(logMock.Object);
            internalContainerMock.Setup(x => x.Get<int>(contextMock.Object)).Returns(1);
            Assert.AreEqual(1, container.Get<int>());

            logMock.Setup(x => x.GetLog()).Returns("zzz");
            Assert.AreEqual("zzz", container.LastConstructionLog);
        }

        private Container container;
        private Mock<IInternalContainer> internalContainerMock;
        private Mock<IContextHolder> holderMock;
        private Mock<IInjectionContext> contextMock;
        private Mock<IGroboContainerLog> logMock;
    }
}