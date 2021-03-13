using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class FuncBuilderTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            funcBuilder = new FuncBuilder();
            contextMock = GetMock<IInjectionContext>();
            containerMock = GetMock<IContainerForFuncBuilder>();
        }

        [Test]
        public void TestGet()
        {
            var func = funcBuilder.BuildGetFunc<int>(contextMock.Object);
            contextMock.Setup(x => x.ContainerForFunc).Returns(containerMock.Object);
            containerMock.Setup(x => x.GetForFunc<int>()).Returns(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestNoArgs()
        {
            var func = funcBuilder.BuildCreateFunc<int>(contextMock.Object);
            contextMock.Setup(x => x.ContainerForFunc).Returns(containerMock.Object);
            containerMock.Setup(x => x.CreateForFunc<int>()).Returns(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestOneArg()
        {
            var func = funcBuilder.BuildCreateFunc<string, int>(contextMock.Object);
            contextMock.Setup(x => x.ContainerForFunc).Returns(containerMock.Object);
            containerMock.Setup(x => x.CreateForFunc<string, int>("q")).Returns(1);
            Assert.AreEqual(1, func("q"));
        }

        private IFuncBuilder funcBuilder;
        private Mock<IInjectionContext> contextMock;
        private Mock<IContainerForFuncBuilder> containerMock;
    }
}