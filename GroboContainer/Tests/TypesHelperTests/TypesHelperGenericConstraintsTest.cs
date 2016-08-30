using NUnit.Framework;

namespace Tests.TypesHelperTests
{
    public class TypesHelperGenericConstraintsTest : TypesHelperGenericTestBase
    {
        private interface IConstraint {}

        private class ConcreteEntity1 {}

        private class ConcreteEntity2 :IConstraint {}

        private interface IAbstract<T> {}

        private class GenericClass<T> : IAbstract<T> {}

        private class GenericClassWithConstraint<T> : IAbstract<T> where T: IConstraint {}

        [Test]
        public void Test()
        {
            CheckFalse<IAbstract<ConcreteEntity1>>(typeof(GenericClassWithConstraint<>));
        }
    }
}