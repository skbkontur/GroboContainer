using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class ReuseImplementationsWithExplicitUseTypeConfigurationTest : ContainerTestBase
    {
        public interface IInterface
        {
        }

        [Test]
        public void TestBug()
        {
            container.Configurator.ForAbstraction<IInterface>().UseType<Impl>();
            Assert.That(container.Get<IInterface>(), Is.SameAs(container.Get<Impl>()));
        }

        public class Impl : IInterface
        {
        }
    }
}