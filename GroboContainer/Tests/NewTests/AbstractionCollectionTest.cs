using GroboContainer.New;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.NewTests
{
    public class AbstractionCollectionTest : TestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            implementationTypesCollection = GetMock<IImplementationTypesCollection>();
            implementationCache = GetMock<IImplementationCache>();
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, implementationCache);
        }

        #endregion

        private IImplementationTypesCollection implementationTypesCollection;
        private IImplementationCache implementationCache;
        private AbstractionsCollection abstractionsCollection;

        [Test]
        public void TestStupid()
        {
            var implTypes = new[] {typeof (string), typeof (byte)};
            implementationTypesCollection.Expect(mock => mock.GetImplementationTypes(typeof (int))).Return(implTypes);
            var expectedImpls = new[] {GetMock<IImplementation>(), GetMock<IImplementation>()};
            implementationCache.Expect(mock => mock.GetOrCreate(implTypes[0])).Return(expectedImpls[0]);
            implementationCache.Expect(mock => mock.GetOrCreate(implTypes[1])).Return(expectedImpls[1]);

            IAbstraction abstraction = abstractionsCollection.Get(typeof (int));
            Assert.IsInstanceOfType(typeof (Abstraction), abstraction);
            CollectionAssert.AreEqual(expectedImpls, abstraction.GetImplementations());
            CollectionAssert.AreEqual(expectedImpls, abstraction.GetImplementations());
        }
    }
}