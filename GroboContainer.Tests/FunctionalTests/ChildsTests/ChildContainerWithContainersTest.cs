using System;
using System.Diagnostics;

using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Infection;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests.ChildsTests
{
    public class ChildContainerWithContainersTest : TestBase
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

        private interface IRoot : IDisposable
        {
        }

        [ChildType]
        private interface IChild : IDisposable
        {
        }

        [Test]
        public void TestBadConfigure()
        {
            IContainer childContainerA = container.MakeChildContainer();
            RunMethodWithException<InvalidOperationException>(
                () => childContainerA.Configurator.ForAbstraction<IRoot>().Fail());
            RunMethodWithException<InvalidOperationException>(
                () => container.Configurator.ForAbstraction<IChild>().Fail());
            Assert.That(childContainerA.Get<IChild>(), Is.InstanceOf<Child>());
            Assert.That(container.Get<IRoot>(), Is.InstanceOf<Root>());
        }

        [Test]
        public void TestChildContainersInConstructors()
        {
            IContainer childContainerA = container.MakeChildContainer();
            var childA = childContainerA.Get<IChild>();
            Assert.That(childA, Is.InstanceOf<Child>());
            var childClassA = (Child)childA;

            IContainer childContainerB = container.MakeChildContainer();
            var childB = childContainerB.Get<IChild>();
            Assert.That(childB, Is.InstanceOf<Child>());
            var childClassB = (Child)childB;

            Assert.AreNotSame(childClassA, childClassB);
            Assert.AreSame(childClassA.root, childClassB.root);
            Assert.AreNotSame(childClassA.child2, childClassB.child2);

            Assert.AreSame(childClassA.root, container.Get<IRoot>());

            Assert.IsFalse(childClassA.disposed);
            childClassA.container.Dispose();
            Assert.That(childClassA.disposed);
            Assert.IsFalse(childClassB.disposed);
            Assert.IsFalse(((Root)childClassA.root).disposed);

            container.Dispose();
            Assert.That(childClassA.disposed);
            Assert.IsFalse(childClassB.disposed);
            Assert.That(((Root)childClassA.root).disposed);
        }

        [Test]
        public void TestConfigurationNotSharedBetweenChilds()
        {
            IContainer childContainerA = container.MakeChildContainer();
            IContainer childContainerB = container.MakeChildContainer();
            childContainerA.Configurator.ForAbstraction<Child2>().Fail();
            RunFail<ForbiddenAbstractionException>(() =>
                                                   childContainerA.Get<Child2>());
            childContainerB.Get<Child2>();
        }

        private IContainerSelector selector;
        private IContainer container;

        private class Rot2
        {
        }

        private class Root : IRoot
        {
            public Root(IContainer container)
            {
                container.Get<Rot2>();
            }

            public void Dispose()
            {
                disposed = true;
            }

            public bool disposed;
        }

        [ChildType]
        private class Child2
        {
        }

        private class Child : IChild
        {
            public Child(IContainer container, IRoot root)
            {
                this.container = container;
                this.root = root;
                child2 = container.Get<Child2>();
            }

            public void Dispose()
            {
                disposed = true;
            }

            public readonly Child2 child2;
            public readonly IContainer container;
            public readonly IRoot root;

            public bool disposed;
        }
    }
}