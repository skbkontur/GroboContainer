using System;

using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using GroboContainer.New;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.NewTests
{
    public class AutoImplementationConfigurationTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            creationContext = GetMock<ICreationContext>();

            implementation = GetMock<IImplementation>();
            autoImplementationConfiguration = new AutoImplementationConfiguration(implementation);
        }

        #endregion

        [Test]
        public void TestDisposableInstance()
        {
            var injectionContext = GetMock<IInjectionContext>();

            autoImplementationConfiguration.DisposeInstance();

            implementation.Expect(mock => mock.ObjectType).Return(typeof(int)).Repeat.Any();
            var classFactory = GetMock<IClassFactory>();

            implementation.Expect(context => context.GetFactory(Type.EmptyTypes, creationContext)).Return(classFactory);
            var instance = GetMock<IDisposable>();
            classFactory.Expect(f => f.Create(injectionContext, new object[0])).Return(instance);
            var returnedInstance = autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType());
            Assert.AreSame(instance, returnedInstance);

            injectionContext.Expect(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType()));

            instance.Expect(i => i.Dispose());
            autoImplementationConfiguration.DisposeInstance();

            injectionContext.Expect(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType()));

            instance.Expect(i => i.Dispose());
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
            implementation.Expect(mock => mock.ObjectType).Return(typeof(int)).Repeat.Any();
            var classFactory = GetMock<IClassFactory>();

            implementation.Expect(context => context.GetFactory(parameterTypes, creationContext)).Return(classFactory);
            Assert.AreSame(classFactory, autoImplementationConfiguration.GetFactory(parameterTypes, creationContext));

            var classFactory2 = GetMock<IClassFactory>();
            implementation.Expect(context => context.GetFactory(parameterTypes, creationContext)).Return(classFactory2);
            Assert.AreSame(classFactory2, autoImplementationConfiguration.GetFactory(parameterTypes, creationContext));
        }

        [Test]
        public void TestObjectType()
        {
            implementation.Expect(mock => mock.ObjectType).Return(typeof(int));
            Assert.AreEqual(typeof(int), autoImplementationConfiguration.ObjectType);
        }

        [Test]
        public void TestWorkWithNonDisposableInstance()
        {
            var injectionContext = GetMock<IInjectionContext>();

            implementation.Expect(mock => mock.ObjectType).Return(typeof(int)).Repeat.Any();
            var classFactory = GetMock<IClassFactory>();

            implementation.Expect(context => context.GetFactory(Type.EmptyTypes, creationContext)).Return(classFactory);
            const string instance = "zzz";
            classFactory.Expect(f => f.Create(injectionContext, new object[0])).Return(instance);
            var returnedInstance = autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType());
            Assert.AreSame(instance, returnedInstance);

            injectionContext.Expect(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType()));

            autoImplementationConfiguration.DisposeInstance();

            injectionContext.Expect(ic => ic.Reused(typeof(int)));
            Assert.AreSame(instance,
                           autoImplementationConfiguration.GetOrCreateInstance(injectionContext, creationContext, implementation.GetType()));
        }

        private IImplementation implementation;
        private AutoImplementationConfiguration autoImplementationConfiguration;
        private ICreationContext creationContext;
    }
}