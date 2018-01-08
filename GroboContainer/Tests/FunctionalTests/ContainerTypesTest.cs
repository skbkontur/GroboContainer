using GroboContainer.Core;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerTypesTest : CoreTestBase
    {
        private class C1
        {
        }


        private class C2
        {
            public readonly C1 c1;

            public C2(C1 c1)
            {
                this.c1 = c1;
            }
        }

        [Test]
        public void TestGetTypeThatImplementsItself()
        {
            var configuration = new TestConfiguration(new[] {typeof (int)});
            var container = new Container(configuration);
            Assert.IsNotNull(container.Get<C1>());
        }

        [Test]
        public void TestGetTypeWithDependencies()
        {
            var configuration = new TestConfiguration(new[] {typeof (C1)});
            var container = new Container(configuration);
            var c2 = container.Create<C2>();
            Assert.IsNotNull(c2);
            Assert.IsNotNull(c2.c1);
        }
    }
}