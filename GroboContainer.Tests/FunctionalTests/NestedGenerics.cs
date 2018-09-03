using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class NestedGenerics : ContainerTestBase
    {
        public interface IX<T1>
        {
        }

        public interface IY<T2>
        {
        }

        [Test]
        public void Test()
        {
            Assert.That(container.Get<Wrap>().dependency, Is.InstanceOf<X1<A>>());
        }

        public class A
        {
        }

        public class B<T3>
        {
        }

        public class X1<T4> : IX<T4>
        {
        }

        public class X2<T5> : IX<B<T5>>
        {
        }

        public class Wrap
        {
            public Wrap(IX<A> dependency)
            {
                this.dependency = dependency;
            }

            public readonly IX<A> dependency;
        }
    }
}