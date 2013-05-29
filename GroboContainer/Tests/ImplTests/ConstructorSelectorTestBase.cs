using System;
using System.Reflection;
using GroboContainer.Impl.ClassCreation;
using NUnit.Framework;
using TestCore;

namespace Tests.ImplTests
{
    public abstract class ConstructorSelectorTestBase : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            constructorSelector = new ConstructorSelector();
        }

        #endregion

        protected ConstructorSelector constructorSelector;

        protected static void CheckConstructor<T>(Type[] types, ContainerConstructorInfo constructor,
                                                  int[] expectedPermutation)
        {
            Assert.IsNotNull(constructor.ConstructorInfo, " онструктор не найден");
            Assert.AreEqual(typeof (T), constructor.ConstructorInfo.ReflectedType);
            ConstructorInfo expected = typeof (T).GetConstructor(types);
            Assert.IsNotNull(expected, "ќжидаемый конструктор не найден");
            Assert.AreEqual(expected, constructor.ConstructorInfo, "не тот конструктор");
            CollectionAssert.AreEqual(expectedPermutation, constructor.ParametersInfo);
        }
    }
}