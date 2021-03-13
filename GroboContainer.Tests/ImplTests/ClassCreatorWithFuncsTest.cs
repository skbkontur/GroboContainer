using System;

using GroboContainer.Impl;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class ClassCreatorWithFuncsTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            classCreator = new ClassCreator(new FuncHelper());
            containerMock = GetMock<IInternalContainer>();
            contextMock = GetMock<IInjectionContext>();
        }

        [Test]
        public void TestSimple()
        {
            var classFactory =
                classCreator.BuildFactory(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = typeof(C1).GetConstructor(new[] {typeof(Func<int, I2>)})
                        }, null);
            var c2 = new C2(11, new C3());
            Func<int, I2> func = arg =>
                {
                    Assert.AreEqual(10, arg);
                    return c2;
                };

            contextMock.Setup(x => x.InternalContainer).Returns(containerMock.Object);
            contextMock.Setup(x => x.BeginConstruct(typeof(C1)));
            contextMock.Setup(x => x.EndConstruct(typeof(C1)));
            containerMock.Setup(x => x.BuildCreateFunc<int, I2>(contextMock.Object)).Returns(func);
            var c1 = (C1)classFactory.Create(contextMock.Object, new object[0]);
            Assert.AreSame(c2, c1.i2);
        }

        private ClassCreator classCreator;
        private Mock<IInternalContainer> containerMock;
        private Mock<IInjectionContext> contextMock;

        // ReSharper disable UnusedMember.Local
        private interface I1
        {
        }

        private interface I2
        {
        }

        private interface I3
        {
        }

        private class C3 : I3
        {
        }

        private class C2 : I2
        {
            public C2(int a, I3 service)
            {
            }
        }

        private class C1 : I1
        {
            public C1(Func<int, I2> createI2)
            {
                i2 = createI2(10);
            }

            public readonly I2 i2;
        }

        // ReSharper restore UnusedMember.Local
    }
}