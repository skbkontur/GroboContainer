using System;

using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.NewTests
{
    public class AutoImplementationConfigurationTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            creationContextMock = GetMock<ICreationContext>();

            implementationMock = GetMock<IImplementation>();
            autoImplementationConfiguration = new AutoImplementationConfiguration(implementationMock.Object);
        }

        [Test]
        public void TestDisposableInstance()
        {
            var injectionContextMock = GetMock<IInjectionContext>();

            autoImplementationConfiguration.DisposeInstance();

            implementationMock.Setup(mock => mock.ObjectType).Returns(typeof(int));
            var classFactoryMock = GetMock<IClassFactory>();

            implementationMock.Setup(context => context.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            var instanceMock = GetMock<IDisposable>();
            classFactoryMock.Setup(f => f.Create(injectionContextMock.Object, new object[0])).Returns(instanceMock.Object);
            var returnedInstance = autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object);
            Assert.AreSame(instanceMock.Object, returnedInstance);

            injectionContextMock.Setup(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instanceMock.Object,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object));

            instanceMock.Setup(i => i.Dispose());
            autoImplementationConfiguration.DisposeInstance();

            injectionContextMock.Setup(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instanceMock.Object,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object));

            instanceMock.Setup(i => i.Dispose());
            autoImplementationConfiguration.DisposeInstance();
        }

        [Test]
        public void TestDisposeInstanceWhenNoInstance()
        {
            autoImplementationConfiguration.DisposeInstance();
        }

        [Test]
        public void TestGetFactory()
        {
            var parameterTypes = new[] {typeof(long)};
            var classFactoryMock = GetMock<IClassFactory>();

            implementationMock.Setup(context => context.GetFactory(parameterTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            Assert.AreSame(classFactoryMock.Object, autoImplementationConfiguration.GetFactory(parameterTypes, creationContextMock.Object));

            var classFactoryMock2 = GetMock<IClassFactory>();
            implementationMock.Setup(context => context.GetFactory(parameterTypes, creationContextMock.Object)).Returns(classFactoryMock2.Object);
            Assert.AreSame(classFactoryMock2.Object, autoImplementationConfiguration.GetFactory(parameterTypes, creationContextMock.Object));
        }

        [Test]
        public void TestObjectType()
        {
            implementationMock.Setup(mock => mock.ObjectType).Returns(typeof(int));
            Assert.AreEqual(typeof(int), autoImplementationConfiguration.ObjectType);
        }

        [Test]
        public void TestWorkWithNonDisposableInstance()
        {
            var injectionContextMock = GetMock<IInjectionContext>();

            implementationMock.Setup(mock => mock.ObjectType).Returns(typeof(int));
            var classFactoryMock = GetMock<IClassFactory>();

            implementationMock.Setup(context => context.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            const string instance = "zzz";
            classFactoryMock.Setup(f => f.Create(injectionContextMock.Object, new object[0])).Returns(instance);
            var returnedInstance = autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object);
            Assert.AreSame(instance, returnedInstance);

            injectionContextMock.Setup(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object));

            autoImplementationConfiguration.DisposeInstance();

            injectionContextMock.Setup(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContextMock.Object, creationContextMock.Object));
        }

        private Mock<IImplementation> implementationMock;
        private AutoImplementationConfiguration autoImplementationConfiguration;
        private Mock<ICreationContext> creationContextMock;
    }
}