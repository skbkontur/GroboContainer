using System;
using System.Reflection;

using GroboContainer.Impl.Implementations;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests
{
    public class TypesHelperTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            helpers = new TypesHelper();
        }

        #endregion

        private interface IInterface
        {
        }

        private interface IInterface2 : IInterface
        {
        }

        private void CheckTrue<TInterface, TImpl>(Type type)
        {
            Type implementation = helpers.TryGetImplementation(typeof(TInterface), type);
            Assert.AreEqual(typeof(TImpl), implementation);
        }

        private void CheckFalse<TInterface>(Type type)
        {
            Assert.IsNull(helpers.TryGetImplementation(typeof(TInterface), type));
        }

        [Test]
        public void TestIgnoredAbstraction()
        {
            var provider = NewMock<ICustomAttributeProvider>();
            provider.ExpectIsDefined<IgnoredAbstractionAttribute>(true);
            Assert.IsTrue(helpers.IsIgnoredAbstraction(provider));
        }

        [Test]
        public void TestIgnoredAbstractionFalse()
        {
            var provider = NewMock<ICustomAttributeProvider>();
            provider.ExpectIsDefined<IgnoredAbstractionAttribute>(false);
            Assert.IsFalse(helpers.IsIgnoredAbstraction(provider));
        }

        [Test]
        public void TestIsIgnoredImplementationFalse()
        {
            var provider = NewMock<ICustomAttributeProvider>();
            provider.ExpectIsDefined<IgnoredImplementationAttribute>(false);
            Assert.IsFalse(helpers.IsIgnoredImplementation(provider));
        }

        [Test]
        public void TestIsIgnoredImplementationTrue()
        {
            var provider = NewMock<ICustomAttributeProvider>();
            provider.ExpectIsDefined<IgnoredImplementationAttribute>(true);
            Assert.IsTrue(helpers.IsIgnoredImplementation(provider));
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