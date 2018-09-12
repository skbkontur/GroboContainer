using System;

using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.ChildContainerSupportTests
{
    public class AttributedChildSelectorTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            selector = new AttributedChildSelector();
        }

        #endregion

        [ChildType]
        private interface IChild
        {
        }

        [Test]
        public void TestAtDepth0()
        {
            Assert.AreEqual(0, selector.Select(typeof(Root), 0));
            RunMethodWithException<InvalidOperationException>(() =>
                                                              selector.Select(typeof(IChild), 0));
        }

        [Test]
        public void TestAtDepth1()
        {
            Assert.AreEqual(0, selector.Select(typeof(Root), 1));
            Assert.AreEqual(1, selector.Select(typeof(IChild), 1));
        }

        [Test]
        public void TestAtDepthGreaterThan1()
        {
            RunMethodWithException<NotSupportedException>(() =>
                                                          selector.Select(typeof(Root), 2));
            RunMethodWithException<NotSupportedException>(() =>
                                                          selector.Select(typeof(IChild), 2));
        }

        private AttributedChildSelector selector;

        private class Root
        {
        }
    }
}