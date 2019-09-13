using System;

using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;

using NMock2;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class InstanceImplementationConfigurationTest : CoreTestBase
    {
        [Test]
        public void TestDispose()
        {
            var disposable = NewMock<IDisposable>();
            var configuration = new InstanceImplementationConfiguration(new TestClassWrapperCreator(), disposable);
            Expect.Once.On(disposable).Method("Dispose");
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
            var context = NewMock<IInjectionContext>();
            context.ExpectReused(instance.GetType());
            Assert.AreSame(instance, configuration.GetOrCreateInstance(context, null, typeof(object)));
        }
    }
}