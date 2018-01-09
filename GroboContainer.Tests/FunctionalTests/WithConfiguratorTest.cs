using System;
using System.Reflection;
using System.Reflection.Emit;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class WithConfiguratorTest : ContainerTestBase
    {
        private interface I1
        {
        }

        private class C2 : I1
        {
        }

        private class C1 : I1
        {
        }

        private class C3 : I1
        {
        }

        private interface IX1
        {
        }

        private interface IX2
        {
        }

        private class X1 : IX1, IX2
        {
        }

        [IgnoredImplementation]
        private class X1Impl2 : IX1, IX2
        {
        }

        [IgnoredAbstraction]
        private interface IIgnored
        {
        }

        public interface INotImplemented
        {
        }

        private static Type CreateProxyType(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException(string.Format("{0} is not an interface", interfaceType.FullName), "interfaceType");

            const string serviceNamePrefix = "DynamicTest";
            var assemblyName = new AssemblyName { Name = serviceNamePrefix + "Assembly" };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var module = assemblyBuilder.DefineDynamicModule(serviceNamePrefix + "Module");

            const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class;
            var typeBuilder = module.DefineType(serviceNamePrefix + "Proxy", typeAttributes, typeof(object), new[] { interfaceType });

            const MethodAttributes constructorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                           MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var constructorBuilder = typeBuilder.DefineConstructor(constructorAttributes, CallingConventions.Standard, Type.EmptyTypes);
            var ilGenerator = constructorBuilder.GetILGenerator();

            var objectConstructor = typeof(Object).GetConstructor(Type.EmptyTypes);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, objectConstructor);
            ilGenerator.Emit(OpCodes.Ret);

            return typeBuilder.CreateType();
        }

        [Test]
        public void TestBadConfig()
        {
            RunMethodWithException<ArgumentException>(() =>
                                                      container.Configurator.ForAbstraction(typeof (I1)).UseInstances(
                                                          "z"));
            RunMethodWithException<ArgumentException>(() =>
                                                      container.Configurator.ForAbstraction(typeof (I1)).UseInstances(
                                                          null));
            RunMethodWithException<ArgumentException>(() =>
                                                      container.Configurator.ForAbstraction(typeof (I1)).UseInstances(
                                                          new object[0]));
        }

        [Test]
        public void TestFail()
        {
            container.Configurator.ForAbstraction(typeof (I1)).Fail();
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Get<I1>());
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Create<I1>());
        }

        [Test]
        public void TestFailGeneric()
        {
            container.Configurator.ForAbstraction<I1>().Fail();
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Get<I1>());
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Create<I1>());
            RunMethodWithException<ForbiddenAbstractionException>(() =>
                                                                  container.GetImplementationTypes(typeof (I1)));
        }

        [Test]
        public void TestIgnoredInterface()
        {
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Get<IIgnored>());
            RunFail<ForbiddenAbstractionException>(() =>
                                                   container.Create<IIgnored>());
        }

        [Test]
        public void TestUseType()
        {
            container.Configurator.ForAbstraction<I1>().UseType<C1>();
            I1[] actual = container.GetAll<I1>();
            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf(typeof(C1), actual[0]);
            Assert.IsInstanceOf(typeof(C1), container.Create<I1>());
            CollectionAssert.AreEquivalent(new[] { typeof(C1)},
                                           container.GetImplementationTypes(typeof(I1)));
            Assert.AreSame(container.Get<I1>(), container.Get<I1>());
        }

        [Test]
        public void TestSetManyInstances()
        {
            var c2 = new C2();
            var c1 = new C1();
            container.Configurator.ForAbstraction<I1>().UseInstances(c1, c2);
            CollectionAssert.AreEquivalent(new I1[] {c2, c1}, container.GetAll<I1>());
            CollectionAssert.AreEquivalent(new[] {typeof (C2), typeof (C1)},
                                           container.GetImplementationTypes(typeof (I1)));
        }

        [Test]
        public void TestSetOneInstance()
        {
            var c2 = new C2();
            container.Configurator.ForAbstraction<I1>().UseInstances(c2);
            Assert.AreSame(c2, container.Get<I1>());
            CollectionAssert.AreEqual(new I1[] {c2}, container.GetAll<I1>());
        }

        [Test]
        public void TestSharingOff()
        {
            var x1 = new X1();
            container.Configurator.ForAbstraction<IX1>().UseInstances(x1);
            Assert.AreSame(x1, container.Get<IX1>());
            var actual = container.Get<IX2>();
            Assert.AreNotSame(x1, actual);
            CollectionAssert.AreEqual(new IX1[] {x1}, container.GetAll<IX1>());
            CollectionAssert.AreEqual(new[] {actual}, container.GetAll<IX2>());
        }

        [Test, Ignore("Не понятно, нужно ли поддержать такую функциональность")]
        public void TestConfigureAfterCreate()
        {
            var x1 = container.Create<IX1>();
            container.Configurator.ForAbstraction<IX1>().UseInstances(x1);
            Assert.AreSame(x1, container.Get<IX1>());
        }

        [Test]
        public void TestGetAutoInstanceAfterCreate()
        {
            var x1Instance1 = container.Create<IX1>();
            var x1Instance2 = container.Get<IX1>();
            Assert.AreNotSame(x1Instance1, x1Instance2);
        }

        [Test, Ignore("Не понятно, нужно ли поддержать такую функциональность")]
        public void TestCreateAfterConfigureInstances()
        {
            var x1 = new X1();
            container.Configurator.ForAbstraction<IX1>().UseInstances(x1);
            Assert.AreNotSame(x1, container.Create<IX1>());
            Assert.AreSame(x1, container.Get<IX1>());
        }

        [Test]
        public void TestCreateWithUseTypeConfiguration()
        {
            container.Configurator.ForAbstraction<IX1>().UseType<X1Impl2>();
            Assert.That(container.Create<IX1>(), Is.InstanceOf<X1Impl2>());
            Assert.That(container.Get<IX1>(), Is.InstanceOf<X1Impl2>());
        }

        [Test]
        public void TestWithDynamicType()
        {
            var implementationTypes = container.GetImplementationTypes(typeof(INotImplemented));
            Assert.AreEqual(0, implementationTypes.Length);

            var proxyType = CreateProxyType(typeof(INotImplemented));
            container.Configurator.ForAbstraction(typeof(INotImplemented)).UseType(proxyType);

            Assert.That(container.Create<INotImplemented>(), Is.InstanceOf(proxyType));
            Assert.That(container.Get<INotImplemented>(), Is.InstanceOf(proxyType));

            Assert.That(container.Create<INotImplemented>(), Is.InstanceOf<INotImplemented>());
            Assert.That(container.Get<INotImplemented>(), Is.InstanceOf<INotImplemented>());

            var proxyType2 = CreateProxyType(typeof(INotImplemented));
            Assert.Throws<InvalidOperationException>(() => container.Configurator.ForAbstraction(typeof (INotImplemented)).UseType(proxyType2));
        }
    }
}