using System;
using System.Collections.Generic;

using GroboContainer.Impl.Implementations;
using GroboContainer.New;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests
{
    public class ImplementationTypesCollectionTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            helperMock = GetMock<ITypesHelper>();
        }

        private interface I1<T>
        {
        }

        [Test]
        public void TestGet()
        {
            var types = new[] {typeof(int), typeof(long)};
            var abstractionType = typeof(long);
            var typeImpl = typeof(long);

            helperMock.Setup(x => x.IsIgnoredImplementation(types[0])).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, types[0], It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns((Type)null);

            helperMock.Setup(x => x.IsIgnoredImplementation(types[1])).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, types[1], It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns(typeImpl);

            var collection = new ImplementationTypesCollection(types, helperMock.Object);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGeneric()
        {
            var types = new[] {typeof(int)};
            var abstractionType = typeof(I1<int>);
            var typeImpl = typeof(Guid);

            helperMock.Setup(x => x.IsIgnoredImplementation(types[0])).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, types[0], It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns((Type)null);

            var definition = abstractionType.GetGenericTypeDefinition();
            helperMock.Setup(x => x.IsIgnoredImplementation(definition)).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, definition, It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns(typeImpl);

            var collection = new ImplementationTypesCollection(types, helperMock.Object);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestGetGenericInConfig()
        {
            var types = new[] {typeof(I1<>)};
            var abstractionType = typeof(I1<int>);
            var typeImpl = abstractionType;

            helperMock.Setup(x => x.IsIgnoredImplementation(types[0])).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, types[0], It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns(typeImpl);

            var collection = new ImplementationTypesCollection(types, helperMock.Object);
            CollectionAssert.AreEquivalent(new[] {typeImpl}, collection.GetImplementationTypes(abstractionType));
        }

        [Test]
        public void TestNoResult()
        {
            var types = new[] {typeof(int)};
            var abstractionType = typeof(long);

            helperMock.Setup(x => x.IsIgnoredImplementation(types[0])).Returns(false);
            helperMock.Setup(x => x.TryGetImplementation(abstractionType, types[0], It.IsAny<Func<Type, IEnumerable<Type>>>())).Returns((Type)null);
            helperMock.Setup(x => x.IsIgnoredImplementation(abstractionType)).Returns(true);

            var collection = new ImplementationTypesCollection(types, helperMock.Object);
            CollectionAssert.IsEmpty(collection.GetImplementationTypes(abstractionType));
        }

        private Mock<ITypesHelper> helperMock;
    }
}