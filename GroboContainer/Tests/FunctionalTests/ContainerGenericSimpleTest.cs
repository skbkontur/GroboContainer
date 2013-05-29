using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerGenericSimpleTest : ContainerTestBase
    {
        private interface IGenericInterface<T>
        {
        }

        private interface IGenericInterface2<T>
        {
        }

        private class ImplementationClass : IGenericInterface<int>
        {
        }

        private class ImplementationGenericClass<T> : IGenericInterface2<T>
        {
        }

        private class GBase<T>
        {
        }

        private class GImpl<T> : GBase<T>
        {
        }

        [Test]
        public void TestGenericClass()
        {
            var genericInterface = container.Get<IGenericInterface2<int>>();
            Assert.IsInstanceOfType(typeof (ImplementationGenericClass<int>), genericInterface);
        }

        [Test]
        public void TestGetGenericNonAbstract()
        {
            Assert.IsInstanceOfType(typeof (GBase<int>), container.Get<GBase<int>>());
            Assert.IsInstanceOfType(typeof (GImpl<int>), container.Get<GImpl<int>>());
        }

        [Test]
        public void TestSimpleGeneric()
        {
            var genericInterface = container.Get<IGenericInterface<int>>();
            Assert.IsInstanceOfType(typeof (ImplementationClass), genericInterface);
        }
    }
}