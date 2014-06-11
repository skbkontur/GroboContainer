using System;
using System.Collections.Generic;
using System.Linq;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Infection;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerFunctionalTest : ContainerTestBase
    {
        private class ClassWithoutArguments : IInterface
        {
        }

        [IgnoredImplementation]
        private class IgnoredClass : IInterface
        {
        }

        private interface I1
        {
        }

        private interface I2
        {
        }

        private class C12 : I1, I2
        {
        }

        private interface IInterface
        {
        }

        private class Base
        {
        }

        private class Impl : Base
        {
        }

        private class ClassWithArguments
        {
            public ClassWithArguments(ClassWithoutArguments dependency)
            {
                Dependency = dependency;
            }

            public ClassWithoutArguments Dependency { get; private set; }
        }

        private class ClassWithFunc
        {
            public ClassWithFunc(Func<IInterface> getDependency)
            {
                Dependency = getDependency;
            }

            public Func<IInterface> Dependency { get; private set; }
        }

        private class WithArrayArgument
        {
            public WithArrayArgument(IInterfaceManyImpls[] args)
            {
                Args = args;
            }

            public IInterfaceManyImpls[] Args { get; private set; }
        }

        private class WithEnumerableArgument
        {
            public WithEnumerableArgument(IEnumerable<IInterfaceManyImpls> args)
            {
                Args = args;
            }

            public IEnumerable<IInterfaceManyImpls> Args { get; private set; }
        }

        private interface IInterfaceManyImpls
        {
        }

        private class InterfaceManyImpls1 : IInterfaceManyImpls
        {
        }

        private class InterfaceManyImpls2 : IInterfaceManyImpls
        {
        }

        [Test]
        public void TestChildContainersDisabled()
        {
            RunMethodWithException<NotSupportedException>(() =>
                                                          container.MakeChildContainer());
        }

        [Test]
        public void TestConstructorAgruments()
        {
            var instance = container.Get<ClassWithArguments>();
            Assert.IsInstanceOfType(typeof (ClassWithArguments), instance);
            Assert.IsInstanceOfType(typeof (ClassWithoutArguments), instance.Dependency);
        }

        [Test]
        public void TestErrorManyImplementations()
        {
            RunFail<ManyImplementationsException>(() => container.Get<IInterfaceManyImpls>(),
                                                  typeof (IInterfaceManyImpls).Name);
        }


        [Test]
        public void TestFunc()
        {
            var instance = container.Get<ClassWithFunc>();
            Assert.IsInstanceOfType(typeof (Func<IInterface>), instance.Dependency);
            IInterface dependency = instance.Dependency();
            Assert.IsInstanceOfType(typeof (ClassWithoutArguments), dependency);
        }

        [Test]
        public void TestGetLazyFunc_Static()
        {
            var func = container.GetLazyFunc<I1>();
            var instance = func();
            Assert.IsInstanceOf<C12>(instance);
        }

        [Test]
        public void TestGetLazyFunc_Dynamic()
        {
            var func = (Func<I1>) container.GetLazyFunc(typeof(Func<I1>));
            var instance = func();
            Assert.IsInstanceOf<C12>(instance);
        }

        [Test]
        public void TestGetCreationFunc_Static()
        {
            var func = container.GetCreationFunc<I1>();
            var instance1 = func();
            var instance2 = func();
            Assert.IsInstanceOf<C12>(instance1);
            Assert.IsInstanceOf<C12>(instance2);
            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestGetCreationFunc_Dynamic()
        {
            var func = (Func<I1>)container.GetCreationFunc(typeof(Func<I1>));
            var instance1 = func();
            var instance2 = func();
            Assert.IsInstanceOf<C12>(instance1);
            Assert.IsInstanceOf<C12>(instance2);
            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestGetIgnoredClass()
        {
            RunFail<NoImplementationException>(() => container.Get<IgnoredClass>());
        }

        [Test]
        public void TestGetImplementationTypes()
        {
            Type[] types = container.GetImplementationTypes(typeof (IInterface));
            CollectionAssert.AreEquivalent(new[] {typeof (ClassWithoutArguments)}, types);
        }

        [Test]
        public void TestGetNonAbstract()
        {
            Assert.IsInstanceOfType(typeof (Base), container.Get<Base>());
            Assert.IsInstanceOfType(typeof (Impl), container.Get<Impl>());
        }

        [Test]
        public void TestShareInstances()
        {
            var instance1 = container.Get<I1>();
            var instance2 = container.Get<I2>();
            Assert.IsInstanceOfType(typeof (C12), instance1);
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestSimple()
        {
            var instance = container.Get<ClassWithoutArguments>();
            Assert.IsInstanceOfType(typeof (ClassWithoutArguments), instance);
        }

        [Test]
        public void TestWithArrayArguments()
        {
            var instance = container.Get<WithArrayArgument>();
            CollectionAssert.AreEquivalent(new[] {typeof (InterfaceManyImpls1), typeof (InterfaceManyImpls2)},
                                           instance.Args.Select(impls => impls.GetType()).ToArray());
        }

        [Test]
        public void TestWithEnumerableArguments()
        {
            var instance = container.Get<WithEnumerableArgument>();
            CollectionAssert.AreEquivalent(new[] {typeof (InterfaceManyImpls1), typeof (InterfaceManyImpls2)},
                                           instance.Args.Select(impls => impls.GetType()).ToArray());
        }

        [Test]
        public void TestWithInterface()
        {
            var instance = container.Get<IInterface>();
            Assert.IsInstanceOfType(typeof (ClassWithoutArguments), instance);
        }
    }
}