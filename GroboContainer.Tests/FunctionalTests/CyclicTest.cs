using System;

using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class CyclicTest : ContainerTestBase
    {
        private interface I1
        {
        }

        private interface I2
        {
        }

        private interface I3
        {
        }

        private interface I4
        {
        }

        [Test]
        public void Test()
        {
            RunFail<CyclicDependencyException>(() =>
                                               container.Get<MyClass1>());
        }

        [Test]
        public void TestGetImplementationTypes()
        {
            var types = container.GetImplementationTypes(typeof(MyClass1));
            CollectionAssert.AreEquivalent(new[] {typeof(MyClass1)}, types);
        }

        [Test]
        public void TestLongCycle()
        {
            RunFail<CyclicDependencyException>(() => container.Get<I4>());
        }

        [Test]
        public void TestWithFunc()
        {
            RunMethodWithException<ContainerException>(
                () => container.Get<MyClass3>(),
                exception =>
                Assert.That(exception.InnerException, Is.InstanceOf<CyclicDependencyException>()));
        }

        [Test]
        public void TestWithFunc2()
        {
            var cycle = container.Get<MyClass3NoCycle>();
            Assert.AreSame(cycle, cycle.func());
        }

        private class MyClass1
        {
            public MyClass1(MyClass2 myClass2)
            {
            }
        }

        private class MyClass2
        {
            public MyClass2(MyClass1 myClass1)
            {
            }
        }

        private class MyClass3
        {
            public MyClass3(Func<MyClass3> getFunc)
            {
                getFunc();
            }
        }

        private class MyClass3NoCycle
        {
            public MyClass3NoCycle(Func<MyClass3NoCycle> getFunc)
            {
                func = getFunc;
            }

            public readonly Func<MyClass3NoCycle> func;
        }

        private class C1 : I1
        {
            public C1(I2 i2)
            {
            }
        }

        private class C2 : I2
        {
            public C2(I3 i3)
            {
            }
        }

        private class C3 : I3
        {
            public C3(I1 i1)
            {
            }
        }

        private class C4 : I4
        {
            public C4(I1 i1)
            {
            }
        }
    }
}