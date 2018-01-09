using System;
using System.Diagnostics;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests.ChildsTests
{
    public class ChildContainerTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            var configuration = new ContainerConfiguration(new[] {GetType().Assembly});
            selector = new RootAndChildSelector();
            container = Container.CreateWithChilds(configuration, null, selector);
        }

        public override void TearDown()
        {
            Debug.WriteLine(container.LastConstructionLog);
            base.TearDown();
        }

        #endregion

        private IContainerSelector selector;
        private IContainer container;

        [RootType]
        private class Root
        {
        }

        [ChildType]
        private class Child
        {
            public readonly Root r;

            public Child(Root r)
            {
                this.r = r;
            }
        }

        [RootType]
        private class BadRoot
        {
            public BadRoot(Child c)
            {
            }
        }

        [RootType]
        private class BadRootUsesFunc
        {
            public readonly Func<Child> getC;

            public BadRootUsesFunc(Func<Child> getC)
            {
                this.getC = getC;
            }
        }

        [ChildType]
        private class BigChild
        {
            public readonly Func<Child> getChild;
            public readonly Func<Root> getRoot;

            public BigChild(Func<Root> getRoot, Func<Child> getChild)
            {
                this.getRoot = getRoot;
                this.getChild = getChild;
            }
        }

        [RootType]
        private class FactoryRoot
        {
        }

        [ChildType]
        private class FactoryChild
        {
            public readonly int a;

            public FactoryChild(int a)
            {
                this.a = a;
            }
        }

        [ChildType]
        private class ChildWithFuncs
        {
            public readonly Func<int, FactoryChild> createC;
            public readonly Func<FactoryRoot> createRoot;

            public ChildWithFuncs(Func<FactoryRoot> createRoot, Func<int, FactoryChild> createC)
            {
                this.createRoot = createRoot;
                this.createC = createC;
            }
        }

        [Test]
        public void TestChildContainerDepthCanBeOnly1()
        {
            IContainer child = container.MakeChildContainer().MakeChildContainer();
            RunFail<NotSupportedException>(() => child.Get(typeof (int)),
                                           "Контейнеры с глубиной больше 1 не поддерживаются");
        }

        [Test]
        public void TestChildFuncs()
        {
            IContainer childContainerA = container.MakeChildContainer();
            var bigChild = childContainerA.Get<BigChild>();
            Child child = childContainerA.Get<BigChild>().getChild();
            Assert.AreSame(child, childContainerA.Get<Child>());
            Root expected = bigChild.getRoot();
            var root = container.Get<Root>();
            Assert.AreSame(expected, root);

            IContainer childContainerB = container.MakeChildContainer();
            Assert.AreSame(childContainerB.Get<BigChild>().getRoot(), root);
            Assert.AreNotSame(childContainerB.Get<BigChild>().getChild(), child);
        }

        [Test]
        public void TestFactories()
        {
            IContainer childContainer = container.MakeChildContainer();
            var childWithFuncs = childContainer.Create<ChildWithFuncs>();
            Assert.AreEqual(1, childWithFuncs.createC(1).a);
            FactoryRoot root = childWithFuncs.createRoot();
            Assert.AreNotSame(childWithFuncs.createRoot(), root);
            Assert.AreEqual(3, childWithFuncs.createC(3).a);
        }

        [Test]
        public void TestGetImplementationTypes()
        {
            CollectionAssert.AreEquivalent(new[] {typeof (Root)}, container.GetImplementationTypes(typeof (Root)));
            RunMethodWithException<InvalidOperationException>(() =>
                                                              container.GetImplementationTypes(typeof (Child)));


            IContainer childContainer = container.MakeChildContainer();

            CollectionAssert.AreEquivalent(new[] {typeof (Child)}, childContainer.GetImplementationTypes(typeof (Child)));
            CollectionAssert.AreEquivalent(new[] { typeof(Root) }, childContainer.GetImplementationTypes(typeof(Root)));
        }

        [Test]
        public void TestLazyLinkToChild()
        {
            var badRootUsesFunc = container.Get<BadRootUsesFunc>();
            RunMethodWithException<InvalidOperationException>(() =>
                                                              badRootUsesFunc.getC(), "not marked as Root type");
        }

        [Test]
        public void TestRootCannotUseChild()
        {
            RunFail<InvalidOperationException>(() =>
                                               container.Get<BadRoot>(), "not marked as Root type");
        }

        [Test]
        public void TestSimple()
        {
            var root = container.Get<Root>();
            IContainer childContainerA = container.MakeChildContainer();
            var childA = childContainerA.Get<Child>();
            //Debug.WriteLine(childContainerA.LastConstructionLog);
            Assert.AreSame(root, childA.r);
            Assert.AreSame(childA, childContainerA.Get<Child>());


            IContainer childContainerB = container.MakeChildContainer();
            var childB = childContainerB.Get<Child>();
            Assert.AreSame(root, childB.r);
            Assert.AreSame(childB, childContainerB.Get<Child>());

            Assert.AreNotSame(childA, childB);
        }
    }
}