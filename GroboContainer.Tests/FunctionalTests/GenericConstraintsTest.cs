using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    // Mostly this functionality is tested in TypesHelperTests.
    // Here will be just a couple of tests, that check that all works on the top level.
    [TestFixture]
    public class GenericConstraintsTest : ContainerTestBase
    {
        private interface I<T>
        {
        }

        private class X
        {
        }

        private interface J<T>
        {
        }

        private class Y : J<X>
        {
        }

        private class C<T1, T2> : I<T1>
            where T2 : J<T1>
        {
        }

        [Test]
        public void TestGenericConstraints()
        {
            Assert.That(container.Get<I<X>>(), Is.InstanceOf(typeof(C<X, Y>)));
        }

        private interface I1<T>
        {
        }

        private interface I2<T>
        {
        }

        private interface IA
        {
        }

        private class AA : IA
        {
        }

        private class R<T> : I2<X>
            where T : IA
        {

        }

        private class R2<T1, T2> : I1<T1>
            where T2 : I2<T1>
        {
        }

        [Test]
        public void TestResolveGenericFromConstraintsTwoLevels()
        {
            Assert.That(container.Get<I1<X>>(), Is.InstanceOf(typeof(R2<X, R<AA>>)));
        }
    }
}