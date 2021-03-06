﻿using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.New;

using NUnit.Framework;

namespace GroboContainer.Tests.NewTests
{
    public class ImplementationConfigurationCacheTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            implementationCache = new ImplementationConfigurationCache();
        }

        [Test]
        public void TestCacheWorks()
        {
            var implementation = new Implementation(typeof(int));
            var cfg1 = implementationCache.GetOrCreate(implementation);
            Assert.That(cfg1, Is.InstanceOf<AutoImplementationConfiguration>());
            Assert.AreEqual(typeof(int), cfg1.ObjectType);

            Assert.AreSame(cfg1, implementationCache.GetOrCreate(implementation));

            var implementation2 = new Implementation(typeof(long));
            var cfg2 = implementationCache.GetOrCreate(implementation2);
            Assert.AreNotEqual(cfg1, cfg2);

            Assert.AreSame(cfg1, implementationCache.GetOrCreate(implementation));
            Assert.AreSame(cfg2, implementationCache.GetOrCreate(implementation2));
        }

        private IImplementationConfigurationCache implementationCache;
    }
}