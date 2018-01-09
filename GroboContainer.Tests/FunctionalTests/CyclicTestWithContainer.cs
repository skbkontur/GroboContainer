using System;
using GroboContainer.Core;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class CyclicTestWithContainer : ContainerTestBase
    {
        private class MyClass1
        {
            public MyClass1(MyClass2 myClass2)
            {
            }
        }

        private class MyClass2
        {
            public MyClass2(Func<IContainer> getContainer)
            {
                getContainer().Get<MyClass1>();
            }
        }

        private class MyClass3
        {
            public readonly IContainer container;

            public MyClass3(IContainer container)
            {
                this.container = container;
            }
        }

        private class MyClass3NoCycle
        {
            public readonly Func<MyClass3NoCycle> func;

            public MyClass3NoCycle(Func<MyClass3NoCycle> getFunc)
            {
                func = getFunc;
            }
        }

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

        private class C1 : I1
        {
            public C1(I2 i2)
            {
            }
        }

        private class C2 : I2
        {
            public C2(IContainer container)
            {
                container.Get<I3>();
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


        [Test]
        public void Test()
        {
            RunMethodWithException<ContainerException>(
                () => container.Get<MyClass1>(),
                exception =>
                Assert.That(exception.InnerException.InnerException, Is.InstanceOf<CyclicDependencyException>()));
        }

        [Test]
        public void TestLongCycle()
        {
            RunMethodWithException<ContainerException>(
                () => container.Get<I4>(),
                exception =>
                Assert.That(exception.InnerException.InnerException, Is.InstanceOf<CyclicDependencyException>()));
        }

        [Test]
        public void TestNoCycle()
        {
            var class3 = container.Get<MyClass3>();
            Assert.AreSame(class3, class3.container.Get<MyClass3>());
        }

        [Test]
        public void TestWithFunc2()
        {
            var cycle = container.Get<MyClass3NoCycle>();
            Assert.AreSame(cycle, cycle.func());
        }
    }
}