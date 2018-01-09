using GroboContainer.New;

using NUnit.Framework;

namespace GroboContainer.Tests.NewTests
{
    public class ImplementationCacheTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            implementationCache = new ImplementationCache();
        }

        private ImplementationCache implementationCache;

        [Test]
        public void TestCacheWorks()
        {
            IImplementation implementation = implementationCache.GetOrCreate(typeof (int));
            Assert.That(implementation, Is.InstanceOf<Implementation>());
            Assert.AreEqual(typeof (int), implementation.ObjectType);

            Assert.AreSame(implementation, implementationCache.GetOrCreate(typeof (int)));

            IImplementation implementationLong = implementationCache.GetOrCreate(typeof (long));
            Assert.AreNotEqual(implementation, implementationLong);

            Assert.AreSame(implementation, implementationCache.GetOrCreate(typeof (int)));
            Assert.AreSame(implementationLong, implementationCache.GetOrCreate(typeof (long)));
        }
    }
}