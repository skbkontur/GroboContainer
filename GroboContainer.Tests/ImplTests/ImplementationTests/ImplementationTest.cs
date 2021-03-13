using System;

using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests.ImplementationTests
{
    public class ImplementationTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            creationContextMock = GetMock<ICreationContext>();
            implementationType = typeof(int);
            implementation = new Implementation(implementationType);
        }

        [Test]
        public void TestGetFactoryNoArgs()
        {
            var classFactoryMock = GetMock<IClassFactory>();
            creationContextMock.Setup(context => context.BuildFactory(implementationType, Type.EmptyTypes)).Returns(classFactoryMock.Object);
            Assert.AreSame(classFactoryMock.Object, implementation.GetFactory(Type.EmptyTypes, creationContextMock.Object));
            Assert.AreSame(classFactoryMock.Object, implementation.GetFactory(Type.EmptyTypes, creationContextMock.Object));
        }

        [Test]
        public void TestGetFactoryWithTypes()
        {
            var classFactoryMock1 = GetMock<IClassFactory>();
            var types1 = new[] {typeof(int)};
            creationContextMock.Setup(context => context.BuildFactory(implementationType, types1)).Returns(classFactoryMock1.Object);
            Assert.AreSame(classFactoryMock1.Object, implementation.GetFactory(types1, creationContextMock.Object));
            Assert.AreSame(classFactoryMock1.Object, implementation.GetFactory(types1, creationContextMock.Object));

            var types2 = new[] {typeof(long), typeof(string)};
            var classFactoryMock2 = GetMock<IClassFactory>();
            creationContextMock.Setup(context => context.BuildFactory(implementationType, types2)).Returns(classFactoryMock2.Object);
            Assert.AreSame(classFactoryMock2.Object, implementation.GetFactory(types2, creationContextMock.Object));

            Assert.AreSame(classFactoryMock1.Object, implementation.GetFactory(types1, creationContextMock.Object));
            Assert.AreSame(classFactoryMock2.Object, implementation.GetFactory(types2, creationContextMock.Object));
        }

        [Test]
        public void TestType()
        {
            Assert.AreEqual(typeof(int), implementation.ObjectType);
        }

        private Mock<ICreationContext> creationContextMock;
        private Type implementationType;
        private Implementation implementation;
    }
}