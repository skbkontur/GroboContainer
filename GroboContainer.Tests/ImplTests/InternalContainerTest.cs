using System;

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

using NUnit.Framework;

using Rhino.Mocks;

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
            creationContext = GetMock<ICreationContext>();
            classFactory = GetMock<IClassFactory>();
            var implementationConfigurationCache = GetMock<IImplementationConfigurationCache>();
            var implementationCache = GetMock<IImplementationCache>();
            internalContainer =
                new InternalContainer(new TestContext
                    {
                        AbstractionConfigurationCollection = abstractionConfigurationCollection,
                        Configuration = configuration,
                        ContainerConfigurator = configurator,
                        FuncBuilder = new FuncBuilder(),
                        CreationContext = creationContext,
                        ImplementationConfigurationCache = implementationConfigurationCache,
                        ImplementationCache = implementationCache
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
            var impls1 = new[] {GetMock<IImplementationConfiguration>()};
            var impls2 = new[] {GetMock<IImplementationConfiguration>(), GetMock<IImplementationConfiguration>()};
            configurations[0].ExpectGetImplementations(impls1);
            impls1[0].Expect(impl => impl.DisposeInstance());
            configurations[1].ExpectGetImplementations(impls2);
            impls2[0].Expect(impl => impl.DisposeInstance());
            impls2[1].Expect(impl => impl.DisposeInstance());
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
                var configurations = new[] {GetMock<IImplementationConfiguration>()};
                abstractionConfiguration.ExpectGetImplementations(configurations);
                configurations[0].Expect(c => c.GetFactory(Type.EmptyTypes, creationContext)).Return(classFactory);
                classFactory.Expect(mock => mock.Create(context, new object[0])).Return("instance0");

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
            var configurations = new[] {GetMock<IImplementationConfiguration>()};
            abstractionConfiguration.ExpectGetImplementations(configurations);
            configurations[0].Expect(c => c.GetFactory(Type.EmptyTypes, creationContext)).Return(classFactory);
            classFactory.Expect(mock => mock.Create(context, new object[0])).Return("instance0");

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
                var configurations = new[] {GetMock<IImplementationConfiguration>()};
                abstractionConfiguration.ExpectGetImplementations(configurations);
                configurations[0].Expect(c => c.GetFactory(new[] {typeof(string)}, creationContext)).Return(
                    classFactory);
                classFactory.Expect(mock => mock.Create(context, parameters)).Return("instance0");

                context.ExpectEndCreate(type);
            }
            ordered.Dispose();
            Assert.AreEqual("instance0",
                            internalContainer.Create(type, context, new[] {typeof(string)},
                                                     new object[] {null}));
        }

        [Test]
        public void TestGet()
        {
            var ordered = mockery.Ordered;
            context.ExpectBeginGet(type);
            abstractionConfigurationCollection.ExpectGet(type, abstractionConfiguration);

            var implementationConfiguration = new[] {GetMock<IImplementationConfiguration>()};

            var result = new object[] {"xss"};
            abstractionConfiguration.ExpectGetImplementations(implementationConfiguration);
            implementationConfiguration[0].Expect(
                configuration1 => configuration1.GetOrCreateInstance(context, creationContext)).Return("xss");

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

            var implementationConfiguration = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            abstractionConfiguration.ExpectGetImplementations(implementationConfiguration);
            implementationConfiguration[0].Expect(
                c => c.GetOrCreateInstance(context, creationContext)).Return(1);
            implementationConfiguration[1].Expect(
                c => c.GetOrCreateInstance(context, creationContext)).Return(2);
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

            var implementationConfiguration = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            implementationConfiguration[0].Expect(c => c.ObjectType).Return(typeof(int));
            implementationConfiguration[1].Expect(c => c.ObjectType).Return(typeof(long));
            abstractionConfiguration.ExpectGetImplementations(implementationConfiguration);
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
        private ICreationContext creationContext;
        private IClassFactory classFactory;
    }
}