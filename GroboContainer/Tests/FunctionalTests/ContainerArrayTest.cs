using System;
using System.Linq;
using GroboContainer.Impl.Exceptions;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerArrayTest : ContainerTestBase
    {
        private interface IInterface
        {
        }

        private interface IInterfaceNoImpl
        {
        }

        private class Type1 : IInterface
        {
        }

        private class Type2 : IInterface
        {
        }


        [Test]
        public void TestErrorNone()
        {
            CollectionAssert.IsEmpty(container.GetAll<IInterfaceNoImpl>());
        }

        [Test]
        public void TestGetImplementationTypes()
        {
            Type[] types = container.GetImplementationTypes(typeof (IInterface));
            CollectionAssert.AreEquivalent(new[] {typeof (Type1), typeof (Type2)}, types);
        }

        [Test]
        public void TestMultiple()
        {
            IInterface[] types = container.GetAll<IInterface>();
            CollectionAssert.AreEquivalent(new[] {typeof (Type1), typeof (Type2)},
                                           types.Select(t => t.GetType()).ToArray());
        }

        [Test]
        public void TestSingle()
        {
            object[] types = container.GetAll(typeof (Type1));
            Assert.AreEqual(1, types.Length);
            Assert.IsInstanceOfType(typeof (Type1), types[0]);
        }
    }
}