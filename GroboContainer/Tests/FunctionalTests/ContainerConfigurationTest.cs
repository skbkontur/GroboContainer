using System;
using System.Collections.Generic;
using GroboContainer.Impl;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class ContainerConfigurationTest : CoreTestBase
    {
        [Test]
        public void TestDuplicateAssemblies()
        {
            var configuration = new ContainerConfiguration(GetType().Assembly, GetType().Assembly);
            IEnumerable<Type> types = configuration.GetTypesToScan();
            CollectionAssert.AllItemsAreUnique(types);
            CollectionAssert.Contains(types, GetType());
            CollectionAssert.DoesNotContain(new[] {typeof (int)}, types);
        }
    }
}