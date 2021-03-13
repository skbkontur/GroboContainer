using System;
using System.Linq;

using GroboContainer.Config;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Implementations;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class InternalContainerTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            contextMock = GetMock<IInjectionContext>();

            abstractionConfigurationMock = GetMock<IAbstractionConfiguration>();
            type = typeof(int);
            abstractionConfigurationCollectionMock = GetMock<IAbstractionConfigurationCollection>();
            creationContextMock = GetMock<ICreationContext>();
            classFactoryMock = GetMock<IClassFactory>();
            internalContainer =
                new InternalContainer(new TestContext
                    {
                        AbstractionConfigurationCollection = abstractionConfigurationCollectionMock.Object,
                        Configuration = GetMock<IContainerConfiguration>().Object,
                        ContainerConfigurator = GetMock<IContainerConfigurator>().Object,
                        FuncBuilder = new FuncBuilder(),
                        CreationContext = creationContextMock.Object,
                        ImplementationConfigurationCache = GetMock<IImplementationConfigurationCache>().Object,
                        ImplementationCache = GetMock<IImplementationCache>().Object
                    });
        }

        [Test]
        public void TestBuildCreateFunc()
        {
            var func = internalContainer.BuildCreateFunc<int, string>(contextMock.Object);
            var containerMock = GetMock<IContainerForFuncBuilder>();
            contextMock.Setup(x => x.ContainerForFunc).Returns(containerMock.Object);
            containerMock.Setup(x => x.CreateForFunc<int, string>(1)).Returns("s");
            Assert.AreEqual("s", func(1));
        }

        [Test]
        public void TestBuildGetFunc()
        {
            var func = internalContainer.BuildGetFunc<int>(contextMock.Object);
            var containerMock = GetMock<IContainerForFuncBuilder>();
            contextMock.Setup(x => x.ContainerForFunc).Returns(containerMock.Object);
            containerMock.Setup(x => x.GetForFunc<int>()).Returns(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestCallDispose()
        {
            var configurationMocks = new[] {GetMock<IAbstractionConfiguration>(), GetMock<IAbstractionConfiguration>(), null};
            abstractionConfigurationCollectionMock.Setup(x => x.GetAll()).Returns(configurationMocks.Select(x => x?.Object).ToArray());
            var implMocks1 = new[] {GetMock<IImplementationConfiguration>()};
            var implMocks2 = new[] {GetMock<IImplementationConfiguration>(), GetMock<IImplementationConfiguration>()};
            configurationMocks[0].Setup(x => x.GetImplementations()).Returns(implMocks1.Select(x => x.Object).ToArray());
            implMocks1[0].Setup(impl => impl.DisposeInstance());
            implMocks1[0].Setup(x => x.InstanceCreationOrder).Returns(0);
            configurationMocks[1].Setup(x => x.GetImplementations()).Returns(implMocks2.Select(x => x.Object).ToArray());
            implMocks2[0].Setup(impl => impl.DisposeInstance());
            implMocks2[0].Setup(x => x.InstanceCreationOrder).Returns(0);
            implMocks2[1].Setup(impl => impl.DisposeInstance());
            implMocks2[1].Setup(x => x.InstanceCreationOrder).Returns(0);
            internalContainer.CallDispose();
        }

        [Test]
        public void TestConfigurator()
        {
            var containerConfigurator = internalContainer.Configurator;
            Assert.That(containerConfigurator, Is.InstanceOf<ContainerConfigurator>());
        }

        [Test]
        public void TestCreate()
        {
            contextMock.Setup(x => x.BeginCreate(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(configurationMocks.Select(x => x.Object).ToArray());
            configurationMocks[0].Setup(c => c.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            classFactoryMock.Setup(x => x.Create(contextMock.Object, new object[0])).Returns("instance0");
            contextMock.Setup(x => x.EndCreate(type));
            Assert.AreEqual("instance0", internalContainer.Create(type, contextMock.Object));
        }

        [Test]
        public void TestCreateGeneric()
        {
            var stringType = typeof(string);
            contextMock.Setup(x => x.BeginCreate(stringType));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(stringType)).Returns(abstractionConfigurationMock.Object);
            var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(configurationMocks.Select(x => x.Object).ToArray());
            configurationMocks[0].Setup(c => c.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            classFactoryMock.Setup(x => x.Create(contextMock.Object, new object[0])).Returns("instance0");
            contextMock.Setup(x => x.EndCreate(stringType));
            Assert.AreEqual("instance0", internalContainer.Create<string>(contextMock.Object));
        }

        [Test]
        public void TestCreateNullParameter()
        {
            var parameters = new object[] {null};
            contextMock.Setup(x => x.BeginCreate(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(configurationMocks.Select(x => x.Object).ToArray());
            configurationMocks[0].Setup(c => c.GetFactory(new[] {typeof(string)}, creationContextMock.Object)).Returns(classFactoryMock.Object);
            classFactoryMock.Setup(x => x.Create(contextMock.Object, parameters)).Returns("instance0");
            contextMock.Setup(x => x.EndCreate(type));
            Assert.AreEqual("instance0", internalContainer.Create(type, contextMock.Object, new[] {typeof(string)}, new object[] {null}));
        }

        [Test]
        public void TestGet()
        {
            contextMock.Setup(x => x.BeginGet(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            var implementationConfigurationMocks = new[] {GetMock<IImplementationConfiguration>()};
            var result = new object[] {"xss"};
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            implementationConfigurationMocks[0].Setup(x => x.GetOrCreateInstance(contextMock.Object, creationContextMock.Object)).Returns("xss");
            contextMock.Setup(x => x.EndGet(type));
            Assert.AreSame(result[0], internalContainer.Get(type, contextMock.Object));
        }

        [Test]
        public void TestGetAll()
        {
            contextMock.Setup(x => x.BeginGetAll(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            var implementationConfigurationMocks = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            implementationConfigurationMocks[0].Setup(c => c.GetOrCreateInstance(contextMock.Object, creationContextMock.Object)).Returns(1);
            implementationConfigurationMocks[1].Setup(c => c.GetOrCreateInstance(contextMock.Object, creationContextMock.Object)).Returns(2);
            contextMock.Setup(x => x.EndGetAll(type));
            CollectionAssert.AreEqual(new object[] {1, 2}, internalContainer.GetAll(type, contextMock.Object));
        }

        [Test]
        public void TestGetAllCrash()
        {
            contextMock.Setup(x => x.BeginGetAll(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Throws(new MockException());
            contextMock.Setup(x => x.Crash());
            contextMock.Setup(x => x.EndGetAll(type));
            RunMethodWithException<MockException>(() => internalContainer.GetAll(type, contextMock.Object));
        }

        [Test]
        public void TestGetAllWhenNoImplementations()
        {
            contextMock.Setup(x => x.BeginGetAll(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(new IImplementationConfiguration[0]);
            contextMock.Setup(x => x.EndGetAll(type));
            CollectionAssert.IsEmpty(internalContainer.GetAll(type, contextMock.Object));
        }

        [Test]
        public void TestGetCrash()
        {
            contextMock.Setup(x => x.BeginGet(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Throws(new MockException());
            contextMock.Setup(x => x.Crash());
            contextMock.Setup(x => x.EndGet(type));
            RunMethodWithException<MockException>(() => internalContainer.Get(type, contextMock.Object));
        }

        [Test]
        public void TestGetManyImpls()
        {
            contextMock.Setup(x => x.BeginGet(type));
            abstractionConfigurationCollectionMock.Setup(x => x.Get(type)).Returns(abstractionConfigurationMock.Object);
            var implementationConfigurationMocks = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            implementationConfigurationMocks[0].Setup(c => c.ObjectType).Returns(typeof(int));
            implementationConfigurationMocks[1].Setup(c => c.ObjectType).Returns(typeof(long));
            abstractionConfigurationMock.Setup(x => x.GetImplementations()).Returns(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            contextMock.Setup(x => x.Crash());
            contextMock.Setup(x => x.EndGet(type));
            RunMethodWithException<ManyImplementationsException>(() => internalContainer.Get(type, contextMock.Object));
        }

        private Mock<IAbstractionConfiguration> abstractionConfigurationMock;
        private Type type;
        private InternalContainer internalContainer;
        private Mock<IAbstractionConfigurationCollection> abstractionConfigurationCollectionMock;
        private Mock<IInjectionContext> contextMock;
        private Mock<ICreationContext> creationContextMock;
        private Mock<IClassFactory> classFactoryMock;
    }
}