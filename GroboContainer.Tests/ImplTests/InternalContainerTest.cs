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
            context = NewMock<IInjectionContext>();
            configuration = NewMock<IContainerConfiguration>();

            abstractionConfiguration = NewMock<IAbstractionConfiguration>();
            type = typeof(int);
            abstractionConfigurationCollection = NewMock<IAbstractionConfigurationCollection>();
            configurator = NewMock<IContainerConfigurator>();
            creationContextMock = GetMock<ICreationContext>();
            classFactoryMock = GetMock<IClassFactory>();
            var implementationConfigurationCacheMock = GetMock<IImplementationConfigurationCache>();
            var implementationCacheMock = GetMock<IImplementationCache>();
            internalContainer =
                new InternalContainer(new TestContext
                    {
                        AbstractionConfigurationCollection = abstractionConfigurationCollection,
                        Configuration = configuration,
                        ContainerConfigurator = configurator,
                        FuncBuilder = new FuncBuilder(),
                        CreationContext = creationContextMock.Object,
                        ImplementationConfigurationCache = implementationConfigurationCacheMock.Object,
                        ImplementationCache = implementationCacheMock.Object
                    });
            context = NewMock<IInjectionContext>();
        }

        [Test]
        public void TestBuildCreateFunc()
        {
            var func = internalContainer.BuildCreateFunc<int, string>(context);
            var container = NewMock<IContainerForFuncBuilder>();
            context.ExpectGetContainerForFunc(container);
            container.ExpectCreateForFunc(1, "s");
            Assert.AreEqual("s", func(1));
        }

        [Test]
        public void TestBuildGetFunc()
        {
            var func = internalContainer.BuildGetFunc<int>(context);
            var container = NewMock<IContainerForFuncBuilder>();
            context.ExpectGetContainerForFunc(container);
            container.ExpectGetForFunc(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestCallDispose()
        {
            var configurations = new[]
                {NewMock<IAbstractionConfiguration>(), NewMock<IAbstractionConfiguration>(), null};
            abstractionConfigurationCollection.ExpectGetAll(configurations);
            var implMocks1 = new[] {GetMock<IImplementationConfiguration>()};
            var implMocks2 = new[] {GetMock<IImplementationConfiguration>(), GetMock<IImplementationConfiguration>()};
            configurations[0].ExpectGetImplementations(implMocks1.Select(x => x.Object).ToArray());
            implMocks1[0].Setup(impl => impl.DisposeInstance());
            implMocks1[0].Setup(x => x.InstanceCreationOrder).Returns(0);
            configurations[1].ExpectGetImplementations(implMocks2.Select(x => x.Object).ToArray());
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
            var ordered = mockery.Ordered;
            {
                context.ExpectBeginCreate(type);
                abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);
                var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
                abstractionConfiguration.ExpectGetImplementations(configurationMocks.Select(x => x.Object).ToArray());
                configurationMocks[0].Setup(c => c.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
                classFactoryMock.Setup(mock => mock.Create(context, new object[0])).Returns("instance0");

                context.ExpectEndCreate(type);
            }
            ordered.Dispose();
            Assert.AreEqual("instance0", internalContainer.Create(type, context));
        }

        [Test]
        public void TestCreateGeneric()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginCreate(typeof(string));

            abstractionConfigurationCollection.ExpectGet(typeof(string), abstractionConfiguration);
            var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
            abstractionConfiguration.ExpectGetImplementations(configurationMocks.Select(x => x.Object).ToArray());
            configurationMocks[0].Setup(c => c.GetFactory(Type.EmptyTypes, creationContextMock.Object)).Returns(classFactoryMock.Object);
            classFactoryMock.Setup(mock => mock.Create(context, new object[0])).Returns("instance0");

            context.ExpectEndCreate(typeof(string));
            ordered.Dispose();
            Assert.AreEqual("instance0", internalContainer.Create<string>(context));
        }

        [Test]
        public void TestCreateNullParameter()
        {
            var parameters = new object[] {null};
            var ordered = mockery.Ordered;
            {
                context.ExpectBeginCreate(type);
                abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);
                var configurationMocks = new[] {GetMock<IImplementationConfiguration>()};
                abstractionConfiguration.ExpectGetImplementations(configurationMocks.Select(x => x.Object).ToArray());
                configurationMocks[0].Setup(c => c.GetFactory(new[] {typeof(string)}, creationContextMock.Object)).Returns(classFactoryMock.Object);
                classFactoryMock.Setup(mock => mock.Create(context, parameters)).Returns("instance0");

                context.ExpectEndCreate(type);
            }
            ordered.Dispose();
            Assert.AreEqual("instance0", internalContainer.Create(type, context, new[] {typeof(string)}, new object[] {null}));
        }

        [Test]
        public void TestGet()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGet(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);

            var implementationConfigurationMocks = new[] {GetMock<IImplementationConfiguration>()};

            var result = new object[] {"xss"};
            abstractionConfiguration.ExpectGetImplementations(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            implementationConfigurationMocks[0].Setup(configuration1 => configuration1.GetOrCreateInstance(context, creationContextMock.Object)).Returns("xss");

            context.ExpectEndGet(type);
            ordered.Dispose();
            Assert.AreSame(result[0], internalContainer.Get(type, context));
        }

        [Test]
        public void TestGetAll()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGetAll(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);

            var implementationConfigurationMocks = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            abstractionConfiguration.ExpectGetImplementations(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            implementationConfigurationMocks[0].Setup(c => c.GetOrCreateInstance(context, creationContextMock.Object)).Returns(1);
            implementationConfigurationMocks[1].Setup(c => c.GetOrCreateInstance(context, creationContextMock.Object)).Returns(2);
            context.ExpectEndGetAll(type);
            ordered.Dispose();
            CollectionAssert.AreEqual(new object[] {1, 2}, internalContainer.GetAll(type, context));
        }

        [Test]
        public void TestGetAllCrash()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGetAll(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);
            abstractionConfiguration.ExpectGetImplementations(new MockException());
            context.ExpectCrash();
            context.ExpectEndGetAll(type);
            ordered.Dispose();
            RunMethodWithException<MockException>(() => internalContainer.GetAll(type, context));
        }

        [Test]
        public void TestGetAllWhenNoImplementations()
        {
            context.ExpectBeginGetAll(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);

            var implementationConfiguration = new IImplementationConfiguration[0];
            abstractionConfiguration.ExpectGetImplementations(implementationConfiguration);
            context.ExpectEndGetAll(type);
            CollectionAssert.IsEmpty(internalContainer.GetAll(type, context));
        }

        [Test]
        public void TestGetCrash()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGet(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);
            abstractionConfiguration.ExpectGetImplementations(new MockException());
            context.ExpectCrash();
            context.ExpectEndGet(type);
            ordered.Dispose();
            RunMethodWithException<MockException>(() => internalContainer.Get(type, context));
        }

        [Test]
        public void TestGetManyImpls()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGet(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);

            var implementationConfigurationMocks = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            implementationConfigurationMocks[0].Setup(c => c.ObjectType).Returns(typeof(int));
            implementationConfigurationMocks[1].Setup(c => c.ObjectType).Returns(typeof(long));
            abstractionConfiguration.ExpectGetImplementations(implementationConfigurationMocks.Select(x => x.Object).ToArray());
            context.ExpectCrash();
            context.ExpectEndGet(type);
            ordered.Dispose();
            RunMethodWithException<ManyImplementationsException>(() => internalContainer.Get(type, context));
        }

        private IContainerConfiguration configuration;
        private IAbstractionConfiguration abstractionConfiguration;
        private Type type;
        private InternalContainer internalContainer;
        private IAbstractionConfigurationCollection abstractionConfigurationCollection;
        private IInjectionContext context;
        private static IContainerConfigurator configurator;
        private Mock<ICreationContext> creationContextMock;
        private Mock<IClassFactory> classFactoryMock;
    }
}