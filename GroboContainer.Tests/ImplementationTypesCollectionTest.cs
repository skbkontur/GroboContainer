using System;

using GroboContainer.Impl.Implementations;
using GroboContainer.New;
using GroboContainer.Tests.TypesHelperTests;

using NUnit.Framework;

namespace GroboContainer.Tests
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

        private interface I1<T>
        {
        }

        [Test]
        public void TestGet()
        {
            var types = new[] {typeof(int), typeof(long)};
            var abstractionType = typeof(long);
            var typeImpl = typeof(long);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);

            helper.ExpectIsIgnoredImplementation(types[1], false);
            helper.ExpectTryGetImplementation(abstractionType, types[1], typeImpl);

            var collection = new ImplementationTypesCollection(types, helper);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGeneric()
        {
            var types = new[] {typeof(int)};
            var abstractionType = typeof(I1<int>);
            var typeImpl = typeof(Guid);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);

            var definition = abstractionType.GetGenericTypeDefinition();
            helper.ExpectIsIgnoredImplementation(definition, false);
            helper.ExpectTryGetImplementation(abstractionType, definition, typeImpl);

            var collection = new ImplementationTypesCollection(types, helper);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGenericInConfig()
        {
            var types = new[] {typeof(I1<>)};
            var abstractionType = typeof(I1<int>);
            var typeImpl = abstractionType;

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], typeImpl);

            var collection = new ImplementationTypesCollection(types, helper);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestNoResult()
        {
            var types = new[] {typeof(int)};
            var abstractionType = typeof(long);

            helper.ExpectIsIgnoredImplementation(types[0], false);
            helper.ExpectTryGetImplementation(abstractionType, types[0], null);
            helper.ExpectIsIgnoredImplementation(abstractionType, true);

            var collection = new ImplementationTypesCollection(types, helper);
            CollectionAssert.IsEmpty(collection.GetImplementationTypes(abstractionType));
        }

        private ITypesHelper helper;
    }
}