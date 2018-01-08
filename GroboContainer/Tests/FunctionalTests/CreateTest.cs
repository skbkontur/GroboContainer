using System;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class CreateTest : ContainerTestBase
    {
        private interface I1
        {
        }

        private interface I2
        {
        }

        public class C2 : I2
        {
        }

        private class C1 : I1
        {
            public readonly int a;
            public readonly long b;

            public C1(int a, long b, I2 i)
            {
                this.a = a;
                this.b = b;
            }
        }

        public struct C3
        {
            public int a;
        }

        [Test]
        public void TestCreate()
        {
            object obj = container.Create(typeof (I1), new[] {typeof (long), typeof (int)}, new object[] {1L, 2});
            Assert.That(obj, Is.InstanceOf<C1>());
            Assert.AreEqual(1, ((C1) obj).b);
            Assert.AreEqual(2, ((C1) obj).a);
        }

        [Test]
        public void TestCreateGeneric()
        {
            object obj = container.Create<long, int, I1>(1, 2);
            Assert.That(obj, Is.InstanceOf<C1>());
            Assert.AreEqual(1, ((C1) obj).b);
            Assert.AreEqual(2, ((C1) obj).a);
        }

        [Test]
        public void TestCreateWithBadType()
        {
            RunFail<InvalidCastException>(() => container.Create(typeof (I1),
                                                                 new[] {typeof (long), typeof (int)},
                                                                 new object[] {1, 2}));
        }
    }
}