using System;
using System.Linq;

using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ChildContainersSupport;
using GroboContainer.Impl.ChildContainersSupport.Selectors;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ChildContainerSupportTests
{
    public class CompositeCollectionTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            selectorMock = GetMock<IContainerSelector>();
            abstractionConfigurationMock = GetMock<IAbstractionConfiguration>();
        }

        [Test]
        public void TestAddOnlyToLeafCollection()
        {
            var rootToChildCollectionMocks = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(0);
            RunMethodWithException<InvalidOperationException>(() => compositeCollection.Add(typeof(int), abstractionConfigurationMock.Object));
        }

        [Test]
        public void TestBadSelector()
        {
            var rootToChildCollectionMocks = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(2);
            RunMethodWithException<BadSelectorException>(() => compositeCollection.Get(typeof(int)));

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(-1);
            RunMethodWithException<BadSelectorException>(() => compositeCollection.Get(typeof(int)));
        }

        [Test]
        public void TestMakeChildCollection()
        {
            var rootToChildCollectionMocks = new[] {GetMock<IAbstractionConfigurationCollection>()};
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            var abstractionConfigurationCollectionMock = GetMock<IAbstractionConfigurationCollection>();
            var childCollection = compositeCollection.MakeChildCollection(abstractionConfigurationCollectionMock.Object);

            var configMocks = new[] {GetMock<IAbstractionConfiguration>()};
            abstractionConfigurationCollectionMock.Setup(c => c.GetAll()).Returns(configMocks.Select(x => x.Object).ToArray());
            Assert.AreSame(configMocks.Single().Object, childCollection.GetAll().Single());
        }

        [Test]
        public void TestNoItems()
        {
            RunMethodWithException<ArgumentException>(() => new CompositeCollection(null, selectorMock.Object));
            RunMethodWithException<ArgumentException>(() => new CompositeCollection(new IAbstractionConfigurationCollection[0], selectorMock.Object));
        }

        [Test]
        public void TestOneCollection()
        {
            var rootToChildCollectionMocks = new[] {GetMock<IAbstractionConfigurationCollection>()};
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 0)).Returns(0);
            rootToChildCollectionMocks[0].Setup(c => c.Add(typeof(int), abstractionConfigurationMock.Object));
            compositeCollection.Add(typeof(int), abstractionConfigurationMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 0)).Returns(0);
            rootToChildCollectionMocks[0].Setup(c => c.Get(typeof(int))).Returns(abstractionConfigurationMock.Object);
            Assert.AreSame(abstractionConfigurationMock.Object, compositeCollection.Get(typeof(int)));

            var configMocks = new[] {GetMock<IAbstractionConfiguration>()};
            rootToChildCollectionMocks[0].Setup(c => c.GetAll()).Returns(configMocks.Select(x => x.Object).ToArray());
            Assert.AreSame(configMocks.Single().Object, compositeCollection.GetAll().Single());
        }

        [Test]
        public void TestTwoCollections()
        {
            var rootToChildCollectionMocks = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(1);
            rootToChildCollectionMocks[1].Setup(c => c.Add(typeof(int), abstractionConfigurationMock.Object));
            compositeCollection.Add(typeof(int), abstractionConfigurationMock.Object);

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(0);
            rootToChildCollectionMocks[0].Setup(c => c.Get(typeof(int))).Returns(abstractionConfigurationMock.Object);
            Assert.AreSame(abstractionConfigurationMock.Object, compositeCollection.Get(typeof(int)));

            selectorMock.Setup(s => s.Select(typeof(int), 1)).Returns(1);
            rootToChildCollectionMocks[1].Setup(c => c.Get(typeof(int))).Returns(abstractionConfigurationMock.Object);
            Assert.AreSame(abstractionConfigurationMock.Object, compositeCollection.Get(typeof(int)));

            var configMocks = new[] {GetMock<IAbstractionConfiguration>()};
            rootToChildCollectionMocks[1].Setup(c => c.GetAll()).Returns(configMocks.Select(x => x.Object).ToArray());
            Assert.AreSame(configMocks.Single().Object, compositeCollection.GetAll().Single());
        }

        [Test]
        public void TestWorkWithIContainer()
        {
            var rootToChildCollectionMocks = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollectionMocks.Select(x => x.Object).ToArray(), selectorMock.Object);

            rootToChildCollectionMocks[1].Setup(c => c.Add(typeof(IContainer), abstractionConfigurationMock.Object));
            compositeCollection.Add(typeof(IContainer), abstractionConfigurationMock.Object);

            rootToChildCollectionMocks[1].Setup(c => c.Get(typeof(IContainer))).Returns(abstractionConfigurationMock.Object);
            Assert.AreSame(abstractionConfigurationMock.Object, compositeCollection.Get(typeof(IContainer)));
        }

        private Mock<IContainerSelector> selectorMock;
        private Mock<IAbstractionConfiguration> abstractionConfigurationMock;
    }
}