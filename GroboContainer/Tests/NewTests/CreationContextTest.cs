using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.NewTests
{
    public class CreationContextTest : TestBase
    {
        [Test]
        public void TestSimple()
        {
            var constructorSelector = GetMock<IConstructorSelector>();
            var classCreator = GetMock<IClassCreator>();
            var classFactory = GetMock<IClassFactory>();

            var parameterTypes = new[] {typeof (long)};

            var containerConstructorInfo = new ContainerConstructorInfo();
            constructorSelector.Expect(cs => cs.GetConstructor(typeof (int), parameterTypes)).Return(
                containerConstructorInfo);
            classCreator.Expect(creator => creator.BuildFactory(containerConstructorInfo)).Return(classFactory);

            var creationContext = new CreationContext(classCreator, constructorSelector);
            Assert.AreSame(classFactory, creationContext.BuildFactory(typeof (int), parameterTypes));
        }
    }
}