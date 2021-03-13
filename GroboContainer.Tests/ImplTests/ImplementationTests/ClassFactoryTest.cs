using System.Collections.Generic;

using GroboContainer.Impl;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class ClassFactoryTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            internalContainerMock = GetMock<IInternalContainer>();
            injectionContextMock = GetMock<IInjectionContext>();
            injectionContextMock.Setup(c => c.InternalContainer).Returns(internalContainerMock.Object);
        }

        [Test]
        public void TestCrash()
        {
            var args = new object[1];
            var constructedType = typeof(int);
            var classFactory = new ClassFactory(delegate { throw new MockException(); }, constructedType);

            injectionContextMock.Setup(c => c.BeginConstruct(constructedType));
            injectionContextMock.Setup(c => c.Crash());
            injectionContextMock.Setup(c => c.EndConstruct(constructedType));
            RunMethodWithException<MockException>(() => classFactory.Create(injectionContextMock.Object, args));
        }

        [Test]
        public void TestWork()
        {
            var args = new object[1];
            var constructedType = typeof(int);
            var q = new Queue<int>();
            var classFactory =
                new ClassFactory(delegate(IInternalContainer aContainer, IInjectionContext aContext, object[] arg3)
                    {
                        Assert.AreSame(internalContainerMock.Object, aContainer);
                        Assert.AreSame(injectionContextMock.Object, aContext);
                        Assert.AreSame(args, arg3);
                        return q.Dequeue();
                    }, constructedType);

            injectionContextMock.Setup(c => c.BeginConstruct(constructedType));
            injectionContextMock.Setup(c => c.EndConstruct(constructedType));
            q.Enqueue(100);
            Assert.AreEqual(100, classFactory.Create(injectionContextMock.Object, args));

            injectionContextMock.Setup(c => c.BeginConstruct(constructedType));
            injectionContextMock.Setup(c => c.EndConstruct(constructedType));
            q.Enqueue(200);
            Assert.AreEqual(200, classFactory.Create(injectionContextMock.Object, args));
        }

        private Mock<IInternalContainer> internalContainerMock;
        private Mock<IInjectionContext> injectionContextMock;
    }
}