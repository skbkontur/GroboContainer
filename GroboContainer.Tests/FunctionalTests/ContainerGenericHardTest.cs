using System;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
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

        [Test]
        public void TestGet()
        {
            var @interface = container.Get<IGenericInterface<int, string, long, Guid>>();
            Assert.That(@interface, Is.InstanceOf<GenericClassOneArg<long>>());
        }

        [Test]
        public void TestGetWithDependency()
        {
            var @class = container.Get<SimpleClass<long>>();
            Assert.That(@class, Is.InstanceOf<SimpleClass<long>>());
            Assert.That(@class.interf, Is.InstanceOf<GenericClassOneArg<long>>());
        }

        private class GenericClassOneArg<T> : IGenericInheritor2<int, T>
        {
        }

        private class SimpleClass<T>
        {
            public SimpleClass(IGenericInheritor2<int, T> interf)
            {
                this.interf = interf;
            }

            public readonly IGenericInheritor2<int, T> interf;
        }
    }
}