using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;

using NUnit.Framework;

namespace GroboContainer.Tests.NewTests
{
    public class CreationContextTest : TestBase
    {
        [Test]
        public void TestSimple()
        {
            var constructorSelectorMock = GetMock<IConstructorSelector>();
            var classCreatorMock = GetMock<IClassCreator>();
            var classFactoryMock = GetMock<IClassFactory>();

            var parameterTypes = new[] {typeof(long)};

            var containerConstructorInfo = new ContainerConstructorInfo();
            constructorSelectorMock.Setup(cs => cs.GetConstructor(typeof(int), parameterTypes)).Returns(containerConstructorInfo);
            classCreatorMock.Setup(creator => creator.BuildFactory(containerConstructorInfo, null)).Returns(classFactoryMock.Object);

            var creationContext = new CreationContext(classCreatorMock.Object, constructorSelectorMock.Object, null);
            Assert.AreSame(classFactoryMock.Object, creationContext.BuildFactory(typeof(int), parameterTypes));
        }
    }
}