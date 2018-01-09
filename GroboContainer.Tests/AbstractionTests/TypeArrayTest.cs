using System;
using GroboContainer.Impl.Abstractions;
using NUnit.Framework;

namespace Tests.AbstractionTests
{
    public class TypeArrayTest : CoreTestBase
    {
        private static void CheckEquals(TypeArray a, TypeArray b)
        {
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.That(a.Equals(b));
            Assert.That(a.Equals((object) b));
        }

        private static void CheckDiffers(TypeArray a, TypeArray b)
        {
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals((object) b));
        }

        [Test]
        public void TestDiffers()
        {
            Assert.IsFalse(new TypeArray(Type.EmptyTypes).Equals(null));
            Assert.IsFalse(new TypeArray(Type.EmptyTypes).Equals(1));
            CheckDiffers(new TypeArray(Type.EmptyTypes), new TypeArray(new[] {typeof (int)}));
            CheckDiffers(new TypeArray(new[] {typeof (int)}), new TypeArray(new[] {typeof (uint)}));
            CheckDiffers(new TypeArray(new[] {typeof (int)}), new TypeArray(new[] {typeof (int), typeof (int)}));
            CheckDiffers(new TypeArray(new[] {typeof (string), typeof (int)}),
                         new TypeArray(new[] {typeof (int), typeof (string)}));
        }

        [Test]
        public void TestEquals()
        {
            CheckEquals(new TypeArray(Type.EmptyTypes), new TypeArray(new Type[0]));
            CheckEquals(new TypeArray(new[] {typeof (int)}), new TypeArray(new[] {typeof (int)}));
            CheckEquals(new TypeArray(new[] {typeof (int), typeof (string)}),
                        new TypeArray(new[] {typeof (int), typeof (string)}));
        }

        [Test]
        public void TestSameEquals()
        {
            var array = new TypeArray(new[] {typeof (int)});
            CheckEquals(array, array);
        }
    }
}