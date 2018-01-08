using System;
using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Infection;
using NUnit.Framework;

namespace Tests.ChildContainerSupportTests
{
    public class CompositeContainerContextTest : TestBase
    {
        [ChildType]
        private class CChild
        {
        }

        [Test]
        public void TestAbstractionCollectionSharedBetweenContexts()
        {
            var containerSelector = new RootAndChildSelector();
            var compositeContainerContext = new CompositeContainerContext(
                new ContainerConfiguration(GetType().Assembly), null,
                containerSelector);
            Assert.That(compositeContainerContext.AbstractionConfigurationCollection, Is.InstanceOf<CompositeCollection>());

            IContainerContext childContextA = compositeContainerContext.MakeChildContext();
            IClassFactory factoryFromA = childContextA.AbstractionConfigurationCollection.Get(typeof (CChild)).
                GetImplementations()[0].GetFactory(
                    Type.EmptyTypes, compositeContainerContext.CreationContext);

            IContainerContext childContextB = compositeContainerContext.MakeChildContext();
            IClassFactory factoryFromB = childContextB.AbstractionConfigurationCollection.Get(typeof (CChild)).
                GetImplementations()[0].GetFactory(
                    Type.EmptyTypes, compositeContainerContext.CreationContext);
            Assert.AreSame(factoryFromA, factoryFromB);
        }
    }
}