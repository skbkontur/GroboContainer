using GroboContainer.New;

using NUnit.Framework;

using Rhino.Mocks;

namespace GroboContainer.Tests.NewTests
{
    public class AbstractionCollectionTest : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            implementationTypesCollection = GetMock<IImplementationTypesCollection>();
            implementationCache = GetMock<IImplementationCache>();
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, implementationCache);
        }

        [Test]
        public void TestStupid()
        {
            var implTypes = new[] {typeof(string), typeof(byte)};
            implementationTypesCollection.Expect(mock => mock.GetImplementationTypes(typeof(int))).Return(implTypes);
            var expectedImpls = new[] {GetMock<IImplementation>(), GetMock<IImplementation>()};
            implementationCache.Expect(mock => mock.GetOrCreate(implTypes[0])).Return(expectedImpls[0]);
            implementationCache.Expect(mock => mock.GetOrCreate(implTypes[1])).Return(expectedImpls[1]);

            IAbstraction abstraction = abstractionsCollection.Get(typeof(int));
            Assert.That(abstraction, Is.InstanceOf<Abstraction>());
            CollectionAssert.AreEqual(expectedImpls, abstraction.GetImplementations());
            CollectionAssert.AreEqual(expectedImpls, abstraction.GetImplementations());
        }

        private IImplementationTypesCollection implementationTypesCollection;
        private IImplementationCache implementationCache;
        private AbstractionsCollection abstractionsCollection;
    }
}