using System;

using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ChildContainersSupport;
using GroboContainer.Impl.ChildContainersSupport.Selectors;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.ChildContainerSupportTests
{
    public class CompositeCollectionTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            selector = GetMock<IContainerSelector>();
            abstractionConfiguration = GetMock<IAbstractionConfiguration>();
        }

        #endregion

        [Test]
        public void TestAddOnlyToLeafCollection()
        {
            var rootToChildCollections = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            selector.Expect(s => s.Select(typeof(int), 1)).Return(0);
            RunMethodWithException<InvalidOperationException>(() =>
                                                              compositeCollection.Add(typeof(int),
                                                                                      abstractionConfiguration));
        }

        [Test]
        public void TestBadSelector()
        {
            var rootToChildCollections = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            selector.Expect(s => s.Select(typeof(int), 1)).Return(2);
            RunMethodWithException<BadSelectorException>(() => compositeCollection.Get(typeof(int)));

            selector.Expect(s => s.Select(typeof(int), 1)).Return(-1);
            RunMethodWithException<BadSelectorException>(() => compositeCollection.Get(typeof(int)));
        }

        [Test]
        public void TestMakeChildCollection()
        {
            var rootToChildCollections = new[] {GetMock<IAbstractionConfigurationCollection>()};
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            var abstractionConfigurationCollection = GetMock<IAbstractionConfigurationCollection>();
            var childCollection = compositeCollection.MakeChildCollection(abstractionConfigurationCollection);

            var configs = new[] {GetMock<IAbstractionConfiguration>()};
            abstractionConfigurationCollection.Expect(c => c.GetAll()).Return(configs);
            Assert.AreSame(configs, childCollection.GetAll());
        }

        [Test]
        public void TestNoItems()
        {
            RunMethodWithException<ArgumentException>(() => new CompositeCollection(null, selector));
            RunMethodWithException<ArgumentException>(
                () => new CompositeCollection(new IAbstractionConfigurationCollection[0], selector));
        }

        [Test]
        public void TestOneCollection()
        {
            var rootToChildCollections = new[] {GetMock<IAbstractionConfigurationCollection>()};
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            selector.Expect(s => s.Select(typeof(int), 0)).Return(0);
            rootToChildCollections[0].Expect(c => c.Add(typeof(int), abstractionConfiguration));
            compositeCollection.Add(typeof(int), abstractionConfiguration);

            selector.Expect(s => s.Select(typeof(int), 0)).Return(0);
            rootToChildCollections[0].Expect(c => c.Get(typeof(int))).Return(abstractionConfiguration);
            Assert.AreSame(abstractionConfiguration, compositeCollection.Get(typeof(int)));

            var configs = new[] {GetMock<IAbstractionConfiguration>()};
            rootToChildCollections[0].Expect(c => c.GetAll()).Return(configs);
            Assert.AreSame(configs, compositeCollection.GetAll());
        }

        [Test]
        public void TestTwoCollections()
        {
            var rootToChildCollections = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            selector.Expect(s => s.Select(typeof(int), 1)).Return(1);
            rootToChildCollections[1].Expect(c => c.Add(typeof(int), abstractionConfiguration));
            compositeCollection.Add(typeof(int), abstractionConfiguration);

            selector.Expect(s => s.Select(typeof(int), 1)).Return(0);
            rootToChildCollections[0].Expect(c => c.Get(typeof(int))).Return(abstractionConfiguration);
            Assert.AreSame(abstractionConfiguration, compositeCollection.Get(typeof(int)));

            selector.Expect(s => s.Select(typeof(int), 1)).Return(1);
            rootToChildCollections[1].Expect(c => c.Get(typeof(int))).Return(abstractionConfiguration);
            Assert.AreSame(abstractionConfiguration, compositeCollection.Get(typeof(int)));

            var configs = new[] {GetMock<IAbstractionConfiguration>()};
            rootToChildCollections[1].Expect(c => c.GetAll()).Return(configs);
            Assert.AreSame(configs, compositeCollection.GetAll());
        }

        [Test]
        public void TestWorkWithIContainer()
        {
            var rootToChildCollections = new[]
                {
                    GetMock<IAbstractionConfigurationCollection>(),
                    GetMock<IAbstractionConfigurationCollection>()
                };
            var compositeCollection = new CompositeCollection(rootToChildCollections, selector);

            rootToChildCollections[1].Expect(c => c.Add(typeof(IContainer),
                                                        abstractionConfiguration));
            compositeCollection.Add(typeof(IContainer), abstractionConfiguration);

            rootToChildCollections[1].Expect(c => c.Get(typeof(IContainer))).Return(abstractionConfiguration);
            Assert.AreSame(abstractionConfiguration, compositeCollection.Get(typeof(IContainer)));
        }

        private IContainerSelector selector;
        private IAbstractionConfiguration abstractionConfiguration;
    }
}