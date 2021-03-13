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
            var configurationIntShort = NewMock<IAbstractionConfiguration>();
            var configurationIntInt = NewMock<IAbstractionConfiguration>();
            var configurationLong = NewMock<IAbstractionConfiguration>();
            configurationCollection.Add(typeof(short), configurationIntShort);
            factoryMock.Setup(f => f.CreateByType(typeof(int))).Returns(configurationIntInt);
            Assert.AreSame(configurationIntInt, configurationCollection.Get(typeof(int)));
            Assert.AreSame(configurationIntShort, configurationCollection.Get(typeof(short)));

            factoryMock.Setup(f => f.CreateByType(typeof(long))).Returns(configurationLong);
            Assert.AreSame(configurationLong, configurationCollection.Get(typeof(long)));
        }

        [Test]
        public void TestCreate()
        {
            var configuration = NewMock<IAbstractionConfiguration>();
            factoryMock.Setup(f => f.CreateByType(typeof(int))).Returns(configuration);
            Assert.AreSame(configuration, configurationCollection.Get(typeof(int)));
            Assert.AreSame(configuration, configurationCollection.Get(typeof(int)));
        }

        [Test]
        public void TestGetAll()
        {
            CollectionAssert.IsEmpty(configurationCollection.GetAll());

            var configuration1 = NewMock<IAbstractionConfiguration>();
            var configuration2 = NewMock<IAbstractionConfiguration>();
            configurationCollection.Add(typeof(string), null); //hack, тест на != null
            configurationCollection.Add(typeof(int), configuration1);
            configurationCollection.Add(typeof(long), configuration2);

            CollectionAssert.AreEquivalent(new[] {null, configuration1, configuration2},
                                           configurationCollection.GetAll());
        }

        private Mock<IAutoAbstractionConfigurationFactory> factoryMock;
        private AbstractionConfigurationCollection configurationCollection;
    }
}