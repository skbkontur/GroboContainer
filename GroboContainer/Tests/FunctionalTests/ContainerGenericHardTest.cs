using System;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerGenericHardTest : ContainerTestBase
    {
        private interface IGenericInterface<T1, T2, T3, T4>
        {
        }

        private interface IGenericInheritor1<T1, T2, T3> : IGenericInterface<T3, string, T1, T2>
        {
        }

        private interface IGenericInheritor2<T1, T2> : IGenericInheritor1<T2, Guid, T1>
        {
        }

        private class GenericClassOneArg<T> : IGenericInheritor2<int, T>
        {
        }

        private class SimpleClass<T>
        {
            public readonly IGenericInheritor2<int, T> interf;

            public SimpleClass(IGenericInheritor2<int, T> interf)
            {
                this.interf = interf;
            }
        }

        [Test]
        public void TestGet()
        {
            var @interface = container.Get<IGenericInterface<int, string, long, Guid>>();
            Assert.IsInstanceOfType(typeof (GenericClassOneArg<long>), @interface);
        }

        [Test]
        public void TestGetWithDependency()
        {
            var @class = container.Get<SimpleClass<long>>();
            Assert.IsInstanceOfType(typeof (SimpleClass<long>), @class);
            Assert.IsInstanceOfType(typeof (GenericClassOneArg<long>), @class.interf);
        }
    }
}