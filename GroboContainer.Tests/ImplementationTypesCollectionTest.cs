using System;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;
using NUnit.Framework;
using Tests.TypesHelperTests;

namespace Tests
{
    public class ImplementationTypesCollectionTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            helper = NewMock<ITypesHelper>();
        }

        #endregion

        private ITypesHelper helper;

        private interface I1<T>
        {
        }

        [Test]
        public void TestGet()
        {
            var types = new[] {typeof (int), typeof (long)};
            Type abstractionType = typeof (long);
            Type typeImpl = typeof (long);
            var collection = new ImplementationTypesCollection(new TestConfiguration(types), helper);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);

            helper.ExpectIsIgnoredImplementation(types[1], false);
            helper.ExpectTryGetImplementation(abstractionType, types[1], typeImpl);

            CollectionAssert.AreEquivalent(new[] {typeImpl},
                                           collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGeneric()
        {
            var types = new[] {typeof (int)};
            Type abstractionType = typeof (I1<int>);
            Type typeImpl = typeof (Guid);
            var collection = new ImplementationTypesCollection(new TestConfiguration(types), helper);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);

            Type definition = abstractionType.GetGenericTypeDefinition();
            helper.ExpectIsIgnoredImplementation(definition, false);
            helper.ExpectTryGetImplementation(abstractionType, definition, typeImpl);

            CollectionAssert.AreEquivalent(new[] {typeImpl},
                                           collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGenericInCofig()
        {
            var types = new[] {typeof (I1<>)};
            Type abstractionType = typeof (I1<int>);
            Type typeImpl = abstractionType;
            var collection = new ImplementationTypesCollection(new TestConfiguration(types), helper);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], typeImpl);

            CollectionAssert.AreEquivalent(new[] {typeImpl},
                                           collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestNoResult()
        {
            var types = new[] {typeof (int)};
            Type abstractionType = typeof (long);
            var collection = new ImplementationTypesCollection(new TestConfiguration(types), helper);
            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);

            helper.ExpectIsIgnoredImplementation(abstractionType, true);
            CollectionAssert.IsEmpty(collection.GetImplementationTypes(abstractionType));
        }
    }
}