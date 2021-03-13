using System;

using GroboContainer.Core;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class ContainerImplementationConfigurationTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            configuration = new ContainerImplementationConfiguration();
        }

        [Test]
        public void TestDisposeDoNothing()
        {
            configuration.DisposeInstance();
        }

        [Test]
        public void TestGetFactoryNotSupported()
        {
            RunMethodWithException<NotSupportedException>(() => configuration.GetFactory(Type.EmptyTypes, null));
        }

        [Test]
        public void TestGetInstance()
        {
            var contextMock = GetMock<IInjectionContext>();
            var containerMock = GetMock<IContainer>();
            contextMock.Setup(x => x.Container).Returns(containerMock.Object);
            contextMock.Setup(x => x.Reused(typeof(IContainer)));
            Assert.AreSame(containerMock.Object, configuration.GetOrCreateInstance(contextMock.Object, null));
        }

        private ContainerImplementationConfiguration configuration;
    }
}