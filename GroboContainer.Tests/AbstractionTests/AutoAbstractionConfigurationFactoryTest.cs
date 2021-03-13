using System.Linq;

using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;
using GroboContainer.Tests.TypesHelperTests;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.AbstractionTests
{
    public class AutoAbstractionConfigurationFactoryTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            typesHelper = NewMock<ITypesHelper>();
            abstractionsCollectionMock = GetMock<IAbstractionsCollection>();
            implementationConfigurationCacheMock = GetMock<IImplementationConfigurationCache>();
            factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollectionMock.Object, implementationConfigurationCacheMock.Object);
        }

        [Test]
        public void TestIgnoredAbstraction()
        {
            typesHelper.ExpectIsIgnoredAbstraction(typeof(int), true);
            var configuration = factory.CreateByType(typeof(int));
            Assert.That(configuration, Is.InstanceOf<StupidAbstractionConfiguration>());
            var implementations = configuration.GetImplementations();
            CollectionAssert.AllItemsAreInstancesOfType(implementations, typeof(ForbiddenImplementationConfiguration));
        }

        [Test]
        public void TestOk()
        {
            typesHelper.ExpectIsIgnoredAbstraction(typeof(int), false);
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

        private ITypesHelper typesHelper;
        private AutoAbstractionConfigurationFactory factory;
        private Mock<IAbstractionsCollection> abstractionsCollectionMock;
        private Mock<IImplementationConfigurationCache> implementationConfigurationCacheMock;
    }
}