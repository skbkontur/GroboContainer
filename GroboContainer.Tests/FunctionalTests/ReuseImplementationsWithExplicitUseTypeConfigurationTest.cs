using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class UseTypeTests : ContainerTestBase
    {
        [Test]
        public void ReuseImplementationsWithExplicitUseTypeConfigurationTest()
        {
            container.Configurator.ForAbstraction<IInterface>().UseType<Impl>();
            Assert.That(container.Get<IInterface>(), Is.SameAs(container.Get<Impl>()));
        }

        [Test]
        public void WhenUseTypeConfigurationBindsTwoInterfacesToSingleImplType_ContainerShouldCreateOnlyOneInstanceOfImplType()
        {
            container.Configurator.ForAbstraction<IInterface>().UseType<Impl>();
            container.Configurator.ForAbstraction<IAnotherInterface>().UseType<Impl>();
            Assert.That(container.Get<IInterface>(), Is.SameAs(container.Get<IAnotherInterface>()));
        }

        public interface IInterface
        {
        }

        public interface IAnotherInterface
        {
        }

        public class Impl : IInterface, IAnotherInterface
        {
        }
    }
}