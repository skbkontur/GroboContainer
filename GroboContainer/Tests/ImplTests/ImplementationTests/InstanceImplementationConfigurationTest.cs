using System;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using TestCore;
using TestCore.TestingHelpers;

namespace Tests.ImplTests.ImplementationTests
{
    public class InstanceImplementationConfigurationTest : CoreTestBase
    {
        [Test]
        public void TestDispose()
        {
            var disposable = NewMock<IDisposable>();
            var configuration = new InstanceImplementationConfiguration(disposable);
            disposable.ExpectDispose();
            configuration.DisposeInstance();
        }

        [Test]
        public void TestDisposeDoNothingIfNotDisposable()
        {
            var configuration = new InstanceImplementationConfiguration(new object());
            configuration.DisposeInstance();
        }

        [Test]
        public void TestGetFactoryNotSupported()
        {
            var configuration = new InstanceImplementationConfiguration(new object());
            RunMethodWithException<NotSupportedException>(() => configuration.GetFactory(null, null));
        }

        [Test]
        public void TestGetInstance()
        {
            var instance = new object();
            var configuration = new InstanceImplementationConfiguration(instance);
            var context = NewMock<IInjectionContext>();
            context.ExpectReused(instance.GetType());
            Assert.AreSame(instance, configuration.GetOrCreateInstance(context, null));
        }
    }
}