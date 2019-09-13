using System;

using GroboContainer.Core;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class ContainerImplementationConfigurationTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            configuration = new ContainerImplementationConfiguration();
        }

        #endregion

        [Test]
        public void TestDisposeDoNothing()
        {
            configuration.DisposeInstance();
        }

        [Test]
        public void TestGetFactoryNotSupported()
        {
            RunMethodWithException<NotSupportedException>(() =>
                                                          configuration.GetFactory(Type.EmptyTypes, null));
        }

        [Test]
        public void TestGetInstance()
        {
            var context = NewMock<IInjectionContext>();
            var container = NewMock<IContainer>();
            context.ExpectGetContainer(container);
            context.ExpectReused(typeof(IContainer));
            Assert.AreSame(container, configuration.GetOrCreateInstance(context, null, typeof(IContainer)));
        }

        private ContainerImplementationConfiguration configuration;
    }
}