using GroboContainer.OldHlam;

using NUnit.Framework;

namespace GroboContainer.Tests.AbstractionTests
{
    public class AbstractionKeyTest : CoreTestBase
    {
        private static void CheckTrue(AbstractionKey expected, AbstractionKey key2)
        {
            Assert.IsTrue(expected.Equals(key2));
            Assert.AreEqual(expected.GetHashCode(), key2.GetHashCode());
        }

        private static void CheckFalse(AbstractionKey key1, AbstractionKey key2)
        {
            Assert.IsFalse(key1.Equals(key2));
            Assert.IsFalse(key1.GetHashCode() == key2.GetHashCode());
        }

        [Test]
        public void TestBuild()
        {
            var abstractionKey = new AbstractionKey(typeof (int), new[] {"q", "xx"});
            var abstractionKey1 = new AbstractionKey(typeof (int), new[] {"qx", "x"});
            CheckFalse(abstractionKey, abstractionKey1);
        }

        [Test]
        public void TestCase()
        {
            var abstractionKey = new AbstractionKey(typeof (int), new[] {"A"});
            var abstractionKey1 = new AbstractionKey(typeof (int), new[] {"a"});
            CheckFalse(abstractionKey, abstractionKey1);
        }

        [Test]
        public void TestContractsOrder()
        {
            var contracts = new[] {"qxx", "zqq"};
            var abstractionKey = new AbstractionKey(typeof (int), contracts);
            CollectionAssert.AreEqual(new[] {"qxx", "zqq"}, contracts);


            var strings = new[] {"zqq", "qxx"};
            var abstractionKey1 = new AbstractionKey(typeof (int), strings);
            CollectionAssert.AreEqual(new[] {"zqq", "qxx"}, strings);

            CheckTrue(abstractionKey, abstractionKey1);
        }

        [Test]
        public void TestNullContracts()
        {
            var abstractionKey = new AbstractionKey(typeof (int), null);
            var abstractionKey1 = new AbstractionKey(typeof (int), null);
            var abstractionKey2 = new AbstractionKey(typeof (int), new string[0]);

            CheckTrue(abstractionKey, abstractionKey1);
            CheckTrue(abstractionKey1, abstractionKey);
            CheckFalse(abstractionKey, abstractionKey2);
        }

        [Test]
        public void TestNulls()
        {
            Assert.IsFalse(new AbstractionKey(typeof (int), new string[0]).Equals(null));
        }

        [Test]
        public void TestObjectEqlals()
        {
            Assert.IsFalse(new AbstractionKey(typeof (int), new string[0]).Equals((object) null));
            var key = new AbstractionKey(typeof (int), new string[0]);
            Assert.IsTrue(key.Equals((object) key));
            Assert.IsTrue(key.Equals((object) (new AbstractionKey(typeof (int), new string[0]))));
            Assert.IsFalse(new AbstractionKey(typeof (int), new string[0]).Equals(new object()));
        }

        [Test]
        public void TestSame()
        {
            var abstractionKey = new AbstractionKey(typeof (int), new string[0]);
            CheckTrue(abstractionKey, abstractionKey);
        }

        [Test]
        public void TestType()
        {
            var abstractionKey = new AbstractionKey(typeof (int), new string[0]);
            var abstractionKey1 = new AbstractionKey(typeof (int?), new string[0]);
            CheckFalse(abstractionKey, abstractionKey1);
        }
    }
}