using System;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.ImplTests.ImplementationTests
{
    public class ImplementationTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            creationContext = GetMock<ICreationContext>();
            implementationType = typeof (int);
            implementation = new Implementation(implementationType);
        }

        #endregion

        private ICreationContext creationContext;
        private Type implementationType;
        private Implementation implementation;

        [Test]
        public void TestGetFactoryNoArgs()
        {
            var classFactory = GetMock<IClassFactory>();
            creationContext.Expect(context => context.BuildFactory(implementationType, Type.EmptyTypes)).Return(
                classFactory);
            Assert.AreSame(classFactory, implementation.GetFactory(Type.EmptyTypes, creationContext));
            Assert.AreSame(classFactory, implementation.GetFactory(Type.EmptyTypes, creationContext));
        }

        [Test]
        public void TestGetFactoryWithTypes()
        {
            var classFactory1 = GetMock<IClassFactory>();
            var types1 = new[] {typeof (int)};
            creationContext.Expect(context => context.BuildFactory(implementationType, types1)).Return(classFactory1);
            Assert.AreSame(classFactory1, implementation.GetFactory(types1, creationContext));
            Assert.AreSame(classFactory1, implementation.GetFactory(types1, creationContext));

            var types2 = new[] {typeof (long), typeof (string)};
            var classFactory2 = GetMock<IClassFactory>();
            creationContext.Expect(context => context.BuildFactory(implementationType, types2)).Return(classFactory2);
            Assert.AreSame(classFactory2, implementation.GetFactory(types2, creationContext));

            Assert.AreSame(classFactory1, implementation.GetFactory(types1, creationContext));
            Assert.AreSame(classFactory2, implementation.GetFactory(types2, creationContext));
        }

        [Test]
        public void TestType()
        {
            Assert.AreEqual(typeof (int), implementation.ObjectType);
        }
    }
}