using System;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class UseFactoryFuncTests : ContainerTestBase
    {
        [Test]
        public void TestSimpleGenericAbstractionConfiguration()
        {
            var expectedInstance = new SimpleClass();
            container.Configurator.ForAbstraction<SimpleClass>().UseFactory((c, t) => expectedInstance);
            var instance = container.Get<SimpleClass>();
            Assert.AreSame(expectedInstance, instance);
        }

        [Test]
        public void TestSimpleNonGenericAbstractionConfiguration()
        {
            var expectedInstance = new SimpleClass();
            container.Configurator.ForAbstraction(typeof(SimpleClass)).UseFactory((c, t) => expectedInstance);
            var instance = container.Get<SimpleClass>();
            Assert.AreSame(expectedInstance, instance);
        }

        [Test]
        public void TestGenericClassGenericAbstractionConfiguration()
        {
            var expectedInstance = new GenericClass<SimpleClass>();
            container.Configurator.ForAbstraction<GenericClass<SimpleClass>>().UseFactory((c, t) => expectedInstance);
            var instance = container.Get<GenericClass<SimpleClass>>();
            Assert.AreSame(expectedInstance, instance);
        }

        [Test]
        public void TestActualRequestedTypePassedToFactory()
        {
            var expectedInstance = new GenericClass<SimpleClass>();
            container.Configurator.ForAbstraction(typeof(IGenericInterface<>)).UseFactory((c, t) =>
                {
                    Assert.AreSame(t, typeof(IGenericInterface<SimpleClass>));
                    return expectedInstance;
                });
            var instance = container.Get<IGenericInterface<SimpleClass>>();
            Assert.AreSame(expectedInstance, instance);
        }

        [Test]
        public void TestFactoryResultNotCached()
        {
            container.Configurator.ForAbstraction<SimpleClass>().UseFactory((c, t) => new SimpleClass());
            var instance1 = container.Get<SimpleClass>();
            var instance2 = container.Get<SimpleClass>();
            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void TestCouldResolveAlreadyRegisteredInFactory()
        {
            var expectedString = Guid.NewGuid().ToString();
            container.Configurator.ForAbstraction<string>().UseInstances(expectedString);
            container.Configurator.ForAbstraction<SimpleClass>().UseFactory((c, t) =>
                {
                    var s = container.Get<string>();
                    return new SimpleClass {Value = s};
                });
            var instance = container.Get<SimpleClass>();
            Assert.AreEqual(expectedString, instance.Value);
        }

        private interface IGenericInterface<T>
        {
        }

        private class SimpleClass
        {
            public string Value { get; set; }
        }

        private class GenericClass<T> : IGenericInterface<T>
        {
        }
    }
}