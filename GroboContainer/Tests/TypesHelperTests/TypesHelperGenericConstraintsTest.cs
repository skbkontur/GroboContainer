using NUnit.Framework;

namespace Tests.TypesHelperTests
{
	public class TypesHelperGenericConstraintsTest : TypesHelperGenericTestBase
	{
		private interface IConstraint {}

		private class ConcreteEntity1 {}

		private class ConcreteEntity2 : IConstraint {}

		private interface IAbstract1<T1, T2> {}

		private class GenericClassWithConstraint<T1, T2> : IAbstract1<T2, T1> where T2: IConstraint {}

		[Test]
		public void TestWithFailedConstraint()
		{
			CheckFalse<IAbstract1<ConcreteEntity1, ConcreteEntity2>>(typeof(GenericClassWithConstraint<,>));
		}

		[Test]
		public void TestWithPassedConstraint()
		{
			CheckTrue<IAbstract1<ConcreteEntity2, ConcreteEntity1>, GenericClassWithConstraint<ConcreteEntity1, ConcreteEntity2>>(typeof(GenericClassWithConstraint<,>));
		}
	}
}