using System;
using System.Collections.Generic;

using GroboContainer.Core;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class DisposeTests : ContainerTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            log = new HashSet<string>();
            z1Count = 0;
        }

        private interface I1
        {
        }

        private interface I2
        {
        }

        [Test]
        public void TestBig()
        {
            container.Get<C3>();
            container.Get<C2>();
            container.Dispose();
            CollectionAssert.AreEquivalent(new[] {"C1", "C2"}, log);
            log = new HashSet<string>();
            container.Get<C4>();
            container.Dispose();
            CollectionAssert.AreEquivalent(new[] {"C1", "C2", "C4"}, log);
        }

        [Test]
        public void TestInstanceCreated()
        {
            container.Get<C1>();
            container.Dispose();
            CollectionAssert.AreEquivalent(new[] {"C1"}, log);
            log = new HashSet<string>();
            container.Dispose();
            CollectionAssert.AreEquivalent(new[] {"C1"}, log);
        }

        [Test]
        public void TestInstanceNotCreated()
        {
            container.Dispose();
            CollectionAssert.IsEmpty(log);
        }

        [Test]
        public void TestStupidBehaviourWithConfigurator()
        {
            var c1 = new Z1(); //disposed twice
            container.Configurator.ForAbstraction<C2>().Fail();
            container.Configurator.ForAbstraction<Z1>().UseInstances(c1);
            container.Configurator.ForAbstraction<I1>().UseInstances(c1);
            var foo = container.Get<I2>();
            container.Dispose();
            CollectionAssert.AreEquivalent(new[] {c1.GetHashCode().ToString(), foo.GetHashCode().ToString()}, log);
            Assert.AreEqual(3, z1Count);
        }

        [Test]
        [Repeat(100)]
        public void TestDisposeOrder()
        {
            disposeOrder = new List<string>();
            var a4 = container.Get<A4>();

            container.Dispose();

            var a1Index = disposeOrder.IndexOf("A1");
            var a2Index = disposeOrder.IndexOf("A2");
            var a3Index = disposeOrder.IndexOf("A3");
            var a4Index = disposeOrder.IndexOf("A4");

            Console.WriteLine(string.Join(" ", disposeOrder));

            Assert.That(a1Index, Is.Not.EqualTo(-1));
            Assert.That(a2Index, Is.Not.EqualTo(-1));
            Assert.That(a3Index, Is.Not.EqualTo(-1));
            Assert.That(a4Index, Is.Not.EqualTo(-1));

            Assert.That(a4Index, Is.LessThan(a1Index));
            Assert.That(a4Index, Is.LessThan(a2Index));

            Assert.That(a2Index, Is.LessThan(a1Index));
            Assert.That(a2Index, Is.LessThan(a3Index));
        }

        private static int z1Count;

        private static HashSet<string> log;

        private static List<string> disposeOrder;

        private class C1 : IDisposable
        {
            #region IDisposable Members

            public void Dispose()
            {
                Assert.That(log.Add("C1"));
            }

            #endregion
        }

        private class Z1 : IDisposable, I1, I2
        {
            #region IDisposable Members

            public void Dispose()
            {
                z1Count++;
                log.Add(GetHashCode().ToString());
            }

            #endregion
        }

        private class C2 : IDisposable
        {
            public C2(C1 c1, IContainer container)
            {
            }

            #region IDisposable Members

            public void Dispose()
            {
                Assert.That(log.Add("C2"));
            }

            #endregion
        }

        private class C4 : IDisposable
        {
            #region IDisposable Members

            public void Dispose()
            {
                Assert.That(log.Add("C4"));
            }

            #endregion
        }

        private class C3
        {
            public C3(C1 c1)
            {
            }

            public void Dispose()
            {
                Assert.Fail();
            }
        }

        private class A1 : IDisposable
        {
            public void Dispose()
            {
                disposeOrder.Add("A1");
            }
        }

        private class A3 : IDisposable
        {
            public void Dispose()
            {
                disposeOrder.Add("A3");
            }
        }

        private class A2 : IDisposable
        {
            public A2(A1 a1, A3 a3)
            {
            }

            public void Dispose()
            {
                disposeOrder.Add("A2");
            }
        }

        private class A4 : IDisposable
        {
            public A4(A1 a1, A2 a2)
            {
            }

            public void Dispose()
            {
                disposeOrder.Add("A4");
            }
        }
    }
}