using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;
using NUnit.Framework;

namespace Tests.NewTests
{
    public class ImplementationConfigurationCacheTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            implementationCache = new ImplementationConfigurationCache();
        }

        #endregion

        private IImplementationConfigurationCache implementationCache;

        [Test]
        public void TestCacheWorks()
        {
            var implementation = new Implementation(typeof (int));
            IImplementationConfiguration cfg1 = implementationCache.GetOrCreate(implementation);
            Assert.IsInstanceOfType(typeof (AutoImplementationConfiguration), cfg1);
            Assert.AreEqual(typeof (int), cfg1.ObjectType);

            Assert.AreSame(cfg1, implementationCache.GetOrCreate(implementation));

            var implementation2 = new Implementation(typeof (long));
            IImplementationConfiguration cfg2 = implementationCache.GetOrCreate(implementation2);
            Assert.AreNotEqual(cfg1, cfg2);

            Assert.AreSame(cfg1, implementationCache.GetOrCreate(implementation));
            Assert.AreSame(cfg2, implementationCache.GetOrCreate(implementation2));
        }
    }
}