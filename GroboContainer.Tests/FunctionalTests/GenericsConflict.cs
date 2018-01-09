using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
	public class GenericsConflict : ContainerTestBase
	{
		public interface IX<T1, T2>
		{
		}

		public class A
		{
		}

		public class X1<T3> : IX<T3, T3>
		{
		}

		public class X2<T4> : IX<T4, int>
		{
		}

		[Test]
		public void Test()
		{
			Assert.That(container.Get<IX<string, int>>(), Is.InstanceOf<X2<string>>());
		}
	}
}