using System;
using System.Collections.Generic;
using System.Linq;

using GroboContainer.Impl.Exceptions;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
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

        private class ClassWithFuncDependecy
        {
            public ClassWithFuncDependecy(Func<IInterface> getDependency)
            {
                Dependency = getDependency;
            }

            public Func<IInterface> Dependency { get; private set; }
        }

        private class ClassWithSameFuncDependecy
        {
            public ClassWithSameFuncDependecy(Func<IInterface> getDependency)
            {
                Dependency = getDependency;
            }

            public Func<IInterface> Dependency { get; private set; }
        }

        private class ClassWithLazyDependecy
        {
            public ClassWithLazyDependecy(Lazy<IInterface> dependency)
            {
                Dependency = dependency;
            }

            public Lazy<IInterface> Dependency { get; private set; }
        }

        private class ClassWithSameLazyDependecy
        {
            public ClassWithSameLazyDependecy(Lazy<IInterface> dependency)
            {
                Dependency = dependency;
            }

            public Lazy<IInterface> Dependency { get; private set; }
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
            RunMethodWithException<NotSupportedException>(() => container.MakeChildContainer());
        }

        [Test]
        public void TestConstructorAgruments()
        {
            var instance = container.Get<ClassWithArguments>();
            Assert.That(instance, Is.InstanceOf<ClassWithArguments>());
            Assert.That(instance.Dependency, Is.InstanceOf<ClassWithoutArguments>());
        }

        [Test]
        public void TestErrorManyImplementations()
        {
            RunFail<ManyImplementationsException>(() => container.Get<IInterfaceManyImpls>(), typeof (IInterfaceManyImpls).Name);
        }

        [Test]
        public void TestFuncDependecy()
        {
            var instance = container.Get<ClassWithFuncDependecy>();
            Assert.IsInstanceOf<Func<IInterface>>(instance.Dependency);
            var dependency = instance.Dependency();
            Assert.IsInstanceOf<ClassWithoutArguments>(dependency);

            Assert.That(container.Get<ClassWithFuncDependecy>().Dependency(), Is.SameAs(dependency));
            Assert.That(container.Get<ClassWithSameFuncDependecy>().Dependency(), Is.SameAs(dependency));
        }

        [Test]
        public void TestLazyDependecy()
        {
            var instance = container.Get<ClassWithLazyDependecy>();
            Assert.IsInstanceOf<Lazy<IInterface>>(instance.Dependency);
            var dependency = instance.Dependency.Value;
            Assert.IsInstanceOf<ClassWithoutArguments>(dependency);

            Assert.That(container.Get<ClassWithLazyDependecy>().Dependency.Value, Is.SameAs(dependency));
            Assert.That(container.Get<ClassWithSameLazyDependecy>().Dependency.Value, Is.SameAs(dependency));
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
            Assert.That(container.Get<Base>(), Is.InstanceOf<Base>());
            Assert.That(container.Get<Impl>(), Is.InstanceOf<Impl>());
        }

        [Test]
        public void TestShareInstances()
        {
            var instance1 = container.Get<I1>();
            var instance2 = container.Get<I2>();
            Assert.That(instance1, Is.InstanceOf<C12>());
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void TestSimple()
        {
            var instance = container.Get<ClassWithoutArguments>();
            Assert.That(instance, Is.InstanceOf<ClassWithoutArguments>());
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
            Assert.That(instance, Is.InstanceOf<ClassWithoutArguments>());
        }
    }
}