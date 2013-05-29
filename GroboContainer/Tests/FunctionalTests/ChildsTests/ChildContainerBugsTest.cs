using System;
using System.Diagnostics;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;
using NUnit.Framework;

namespace Tests.FunctionalTests.ChildsTests
{
    public class ChildContainerBugsTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            var configuration = new ContainerConfiguration(new[] {GetType().Assembly});
            selector = new AttributedChildSelector();
            container = Container.CreateWithChilds(configuration, selector);
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        #endregion

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

        [Test]
        public void TestRootCannotUseChildIfGetFromChildContainer()
        {
            IContainer childContainer = container.MakeChildContainer();
            RunMethodWithException<InvalidOperationException>(()=>
                                                              childContainer.Get<Root>());
        }
    }
}