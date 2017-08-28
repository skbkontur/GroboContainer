using System;
using GroboContainer.Impl.Implementations;
using NUnit.Framework;
using TestCore;

namespace Tests.TypesHelperTests
{
    public abstract class TypesHelperGenericTestBase : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            helpers = new TypesHelper();
        }

        #endregion

        private ITypesHelper helpers;

        protected void CheckTrue<TInterface, TImpl>(Type type) where TImpl : TInterface
        {
            Type implementation = helpers.TryGetImplementation(typeof (TInterface), type);
            Assert.AreEqual(typeof (TImpl), implementation);
        }

        protected void CheckTrue<TInterface, TImpl>(Type type, Func<Type, Type[]> getImplementations) where TImpl : TInterface
        {
            Type implementation = helpers.TryGetImplementation(typeof(TInterface), type, getImplementations);
            Assert.AreEqual(typeof(TImpl), implementation);
        }

        protected void CheckFalse<TInterface>(Type type)
        {
            Assert.IsNull(helpers.TryGetImplementation(typeof (TInterface), type));
        }
    }
}