using System.Linq;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.AbstractionTests
{
    public class AutoAbstractionConfigurationFactoryTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            typesHelperMock = GetMock<ITypesHelper>();
            abstractionsCollectionMock = GetMock<IAbstractionsCollection>();
            implementationConfigurationCacheMock = GetMock<IImplementationConfigurationCache>();
            factory = new AutoAbstractionConfigurationFactory(typesHelperMock.Object, abstractionsCollectionMock.Object, implementationConfigurationCacheMock.Object);
        }

        [Test]
        public void TestIgnoredAbstraction()
        {
            typesHelperMock.Setup(x => x.IsIgnoredAbstraction(typeof(int))).Returns(true);
            var configuration = factory.CreateByType(typeof(int));
            Assert.That(configuration, Is.InstanceOf<StupidAbstractionConfiguration>());
            var implementations = configuration.GetImplementations();
            CollectionAssert.AllItemsAreInstancesOfType(implementations, typeof(ForbiddenImplementationConfiguration));
        }

        [Test]
        public void TestOk()
        {
            typesHelperMock.Setup(x => x.IsIgnoredAbstraction(typeof(int))).Returns(false);
            var abstractionMock = GetMock<IAbstraction>();
            abstractionsCollectionMock.Setup(collection => collection.Get(typeof(int))).Returns(abstractionMock.Object);
            var implementationMocks = new[] {GetMock<IImplementation>(), GetMock<IImplementation>()};
            var implementationConfigMocks = new[]
                {
                    GetMock<IImplementationConfiguration>(),
                    GetMock<IImplementationConfiguration>()
                };
            abstractionMock.Setup(abstraction1 => abstraction1.GetImplementations()).Returns(implementationMocks.Select(x => x.Object).ToArray());

            implementationConfigurationCacheMock.Setup(icc => icc.GetOrCreate(implementationMocks[0].Object)).Returns(
                implementationConfigMocks[0].Object);
            implementationConfigurationCacheMock.Setup(icc => icc.GetOrCreate(implementationMocks[1].Object)).Returns(
                implementationConfigMocks[1].Object);

            var configuration = factory.CreateByType(typeof(int));
            Assert.That(configuration, Is.InstanceOf<AutoAbstractionConfiguration>());
            var configurations = configuration.GetImplementations();
            CollectionAssert.AreEqual(implementationConfigMocks.Select(x => x.Object).ToArray(), configurations);
        }

        private Mock<ITypesHelper> typesHelperMock;
        private AutoAbstractionConfigurationFactory factory;
        private Mock<IAbstractionsCollection> abstractionsCollectionMock;
        private Mock<IImplementationConfigurationCache> implementationConfigurationCacheMock;
    }
}