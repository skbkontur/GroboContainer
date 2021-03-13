using System;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.AbstractionTests
{
    public class AbstractionConfigurationCollectionTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            factoryMock = GetMock<IAutoAbstractionConfigurationFactory>();
            configurationCollection = new AbstractionConfigurationCollection(factoryMock.Object);
        }

        [Test]
        public void TestAdd()
        {
            var configurationMock = GetMock<IAbstractionConfiguration>();
            configurationMock
                .Setup(c => c.GetImplementations())
                .Returns(new IImplementationConfiguration[] {new InstanceImplementationConfiguration(null, 1)});

            configurationCollection.Add(typeof(int), configurationMock.Object);
            Assert.AreSame(configurationMock.Object, configurationCollection.Get(typeof(int)));
            Assert.AreSame(configurationMock.Object, configurationCollection.Get(typeof(int)));
            RunMethodWithException<InvalidOperationException>(
                () => configurationCollection.Add(typeof(int), configurationMock.Object),
                "Container is already configured for type System.Int32");
        }

        [Test]
        public void TestBig()
        {
            var configurationIntShortMock = GetMock<IAbstractionConfiguration>();
            var configurationIntIntMock = GetMock<IAbstractionConfiguration>();
            var configurationLongMock = GetMock<IAbstractionConfiguration>();
            configurationCollection.Add(typeof(short), configurationIntShortMock.Object);
            factoryMock.Setup(f => f.CreateByType(typeof(int))).Returns(configurationIntIntMock.Object);
            Assert.AreSame(configurationIntIntMock.Object, configurationCollection.Get(typeof(int)));
            Assert.AreSame(configurationIntShortMock.Object, configurationCollection.Get(typeof(short)));

            factoryMock.Setup(f => f.CreateByType(typeof(long))).Returns(configurationLongMock.Object);
            Assert.AreSame(configurationLongMock.Object, configurationCollection.Get(typeof(long)));
        }

        [Test]
        public void TestCreate()
        {
            var configurationMock = GetMock<IAbstractionConfiguration>();
            factoryMock.Setup(f => f.CreateByType(typeof(int))).Returns(configurationMock.Object);
            Assert.AreSame(configurationMock.Object, configurationCollection.Get(typeof(int)));
            Assert.AreSame(configurationMock.Object, configurationCollection.Get(typeof(int)));
        }

        [Test]
        public void TestGetAll()
        {
            CollectionAssert.IsEmpty(configurationCollection.GetAll());

            var configurationMock1 = GetMock<IAbstractionConfiguration>();
            var configurationMock2 = GetMock<IAbstractionConfiguration>();
            configurationCollection.Add(typeof(string), null); //hack, тест на != null
            configurationCollection.Add(typeof(int), configurationMock1.Object);
            configurationCollection.Add(typeof(long), configurationMock2.Object);

            CollectionAssert.AreEquivalent(new[] {null, configurationMock1.Object, configurationMock2.Object}, configurationCollection.GetAll());
        }

        private Mock<IAutoAbstractionConfigurationFactory> factoryMock;
        private AbstractionConfigurationCollection configurationCollection;
    }
}