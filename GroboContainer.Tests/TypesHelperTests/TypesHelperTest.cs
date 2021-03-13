using System;
using System.Reflection;

using GroboContainer.Impl.Implementations;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests
{
    public class TypesHelperTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            helpers = new TypesHelper();
        }

        private interface IInterface
        {
        }

        private interface IInterface2 : IInterface
        {
        }

        private void CheckTrue<TInterface, TImpl>(Type type)
        {
            var implementation = helpers.TryGetImplementation(typeof(TInterface), type);
            Assert.AreEqual(typeof(TImpl), implementation);
        }

        private void CheckFalse<TInterface>(Type type)
        {
            Assert.IsNull(helpers.TryGetImplementation(typeof(TInterface), type));
        }

        [Test]
        public void TestIgnoredAbstraction()
        {
            var providerMock = GetMock<ICustomAttributeProvider>();
            providerMock.Setup(x => x.IsDefined(typeof(IgnoredAbstractionAttribute), false)).Returns(true);
            Assert.IsTrue(helpers.IsIgnoredAbstraction(providerMock.Object));
        }

        [Test]
        public void TestIgnoredAbstractionFalse()
        {
            var providerMock = GetMock<ICustomAttributeProvider>();
            providerMock.Setup(x => x.IsDefined(typeof(IgnoredAbstractionAttribute), false)).Returns(false);
            Assert.IsFalse(helpers.IsIgnoredAbstraction(providerMock.Object));
        }

        [Test]
        public void TestIsIgnoredImplementationFalse()
        {
            var providerMock = GetMock<ICustomAttributeProvider>();
            providerMock.Setup(x => x.IsDefined(typeof(IgnoredImplementationAttribute), false)).Returns(false);
            Assert.IsFalse(helpers.IsIgnoredImplementation(providerMock.Object));
        }

        [Test]
        public void TestIsIgnoredImplementationTrue()
        {
            var providerMock = GetMock<ICustomAttributeProvider>();
            providerMock.Setup(x => x.IsDefined(typeof(IgnoredImplementationAttribute), false)).Returns(true);
            Assert.IsTrue(helpers.IsIgnoredImplementation(providerMock.Object));
        }

        [Test]
        public void TestIsImplementation()
        {
            CheckTrue<TypesHelperTest, TypesHelperTest>(typeof(TypesHelperTest));
            CheckTrue<IInterface, Impl>(typeof(Impl));
            CheckTrue<MyClass, Impl>(typeof(Impl));
            CheckFalse<MyClass>(typeof(MyClass));
            CheckFalse<IInterface>(typeof(IInterface));
            CheckFalse<IInterface>(typeof(IInterface2));
        }

        [Test]
        public void TestRequestNonAbstractClass()
        {
            CheckFalse<Impl>(typeof(ImplImpl));
        }

        private TypesHelper helpers;

        private class Impl : MyClass
        {
        }

        private class ImplImpl : Impl
        {
        }

        private abstract class MyClass : IInterface
        {
        }
    }
}