using System;

using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class InstanceImplementationConfigurationTest : CoreTestBase
    {
        [Test]
        public void TestDispose()
        {
            var disposableMock = GetMock<IDisposable>();
            var configuration = new InstanceImplementationConfiguration(new TestClassWrapperCreator(), disposableMock.Object);
            disposableMock.Setup(x => x.Dispose());
            configuration.DisposeInstance();
        }

        [Test]
        public void TestDisposeDoNothingIfNotDisposable()
        {
            var configuration = new InstanceImplementationConfiguration(new TestClassWrapperCreator(), new object());
            configuration.DisposeInstance();
        }

        [Test]
        public void TestGetFactoryNotSupported()
        {
            var configuration = new InstanceImplementationConfiguration(new TestClassWrapperCreator(), new object());
            RunMethodWithException<NotSupportedException>(() => configuration.GetFactory(null, null));
        }

        [Test]
        public void TestGetInstance()
        {
            var instance = new object();
            var configuration = new InstanceImplementationConfiguration(new TestClassWrapperCreator(), instance);
            var contextMock = GetMock<IInjectionContext>();
            contextMock.Setup(x => x.Reused(instance.GetType()));
            Assert.AreSame(instance, configuration.GetOrCreateInstance(contextMock.Object, null));
        }
    }
}