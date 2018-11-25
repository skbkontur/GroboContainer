using System;

using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.ChildContainerSupportTests
{
    public class CompositeContainerContextTest : TestBase
    {
        [Test]
        public void TestAbstractionCollectionSharedBetweenContexts()
        {
            var containerSelector = new RootAndChildSelector();
            var compositeContainerContext = new CompositeContainerContext(
                new ContainerConfiguration(GetType().Assembly), null,
                containerSelector);
            Assert.That(compositeContainerContext.AbstractionConfigurationCollection, Is.InstanceOf<CompositeCollection>());

            var childContextA = compositeContainerContext.MakeChildContext();
            var factoryFromA = childContextA.AbstractionConfigurationCollection.Get(typeof(CChild)).
                                             GetImplementations()[0].GetFactory(Type.EmptyTypes, compositeContainerContext.CreationContext);

            var childContextB = compositeContainerContext.MakeChildContext();
            var factoryFromB = childContextB.AbstractionConfigurationCollection.Get(typeof(CChild)).
                                             GetImplementations()[0].GetFactory(Type.EmptyTypes, compositeContainerContext.CreationContext);
            Assert.AreSame(factoryFromA, factoryFromB);
        }

        [ChildType]
        private class CChild
        {
        }
    }
}