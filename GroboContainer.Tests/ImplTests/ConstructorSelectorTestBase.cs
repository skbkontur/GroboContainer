using System;

using GroboContainer.Impl.ClassCreation;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
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

        protected static void CheckConstructor<T>(Type[] types, ContainerConstructorInfo constructor, int[] expectedPermutation)
        {
            Assert.IsNotNull(constructor.ConstructorInfo, "Конструктор не найден");
            Assert.AreEqual(typeof(T), constructor.ConstructorInfo.ReflectedType);
            var expected = typeof(T).GetConstructor(types);
            Assert.IsNotNull(expected, "Ожидаемый конструктор не найден");
            Assert.AreEqual(expected, constructor.ConstructorInfo, "не тот конструктор");
            CollectionAssert.AreEqual(expectedPermutation, constructor.ParametersInfo);
        }

        protected ConstructorSelector constructorSelector;
    }
}