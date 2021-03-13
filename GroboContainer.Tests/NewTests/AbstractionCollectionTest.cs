using System.Linq;

using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.NewTests
{
    public class AbstractionCollectionTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            implementationTypesCollectionMock = GetMock<IImplementationTypesCollection>();
            implementationCacheMock = GetMock<IImplementationCache>();
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollectionMock.Object, implementationCacheMock.Object);
        }

        [Test]
        public void TestStupid()
        {
            var implTypes = new[] {typeof(string), typeof(byte)};
            implementationTypesCollectionMock.Setup(mock => mock.GetImplementationTypes(typeof(int))).Returns(implTypes);
            var expectedImplMocks = new[] {GetMock<IImplementation>(), GetMock<IImplementation>()};
            implementationCacheMock.Setup(mock => mock.GetOrCreate(implTypes[0])).Returns(expectedImplMocks[0].Object);
            implementationCacheMock.Setup(mock => mock.GetOrCreate(implTypes[1])).Returns(expectedImplMocks[1].Object);

            var abstraction = abstractionsCollection.Get(typeof(int));
            Assert.That(abstraction, Is.InstanceOf<Abstraction>());
            CollectionAssert.AreEqual(expectedImplMocks.Select(x => x.Object).ToArray(), abstraction.GetImplementations());
            CollectionAssert.AreEqual(expectedImplMocks.Select(x => x.Object).ToArray(), abstraction.GetImplementations());
        }

        private Mock<IImplementationTypesCollection> implementationTypesCollectionMock;
        private Mock<IImplementationCache> implementationCacheMock;
        private AbstractionsCollection abstractionsCollection;
    }
}