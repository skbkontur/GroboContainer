using GroboContainer.New;
using NUnit.Framework;

namespace Tests.NewTests
{
    public class ImplementationCacheTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            implementationCache = new ImplementationCache();
        }

        #endregion

        private ImplementationCache implementationCache;

        [Test]
        public void TestCacheWorks()
        {
            IImplementation implementation = implementationCache.GetOrCreate(typeof (int));
            Assert.IsInstanceOfType(typeof (Implementation), implementation);
            Assert.AreEqual(typeof (int), implementation.ObjectType);

            Assert.AreSame(implementation, implementationCache.GetOrCreate(typeof (int)));

            IImplementation implementationLong = implementationCache.GetOrCreate(typeof (long));
            Assert.AreNotEqual(implementation, implementationLong);

            Assert.AreSame(implementation, implementationCache.GetOrCreate(typeof (int)));
            Assert.AreSame(implementationLong, implementationCache.GetOrCreate(typeof (long)));
        }
    }
}