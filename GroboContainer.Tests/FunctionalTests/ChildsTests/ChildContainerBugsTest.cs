using System;
using System.Diagnostics;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests.ChildsTests
{
    public class ChildContainerBugsTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            var configuration = new ContainerConfiguration(new[] {GetType().Assembly});
            selector = new AttributedChildSelector();
            container = Container.CreateWithChilds(configuration, null, selector);
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        [Test]
        [Ignore("The status of Child-container feature is not clear")]
        public void TestRootCannotUseChildIfGetFromChildContainer()
        {
            var childContainer = container.MakeChildContainer();
            RunMethodWithException<InvalidOperationException>(() => childContainer.Get<Root>());
        }

        private IContainerSelector selector;
        private IContainer container;

        private class Root
        {
            public Root(Child c)
            {
            }
        }

        [ChildType]
        private class Child
        {
        }
    }
}