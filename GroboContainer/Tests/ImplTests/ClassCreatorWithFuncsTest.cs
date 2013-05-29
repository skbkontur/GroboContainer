using System;
using GroboContainer.Impl;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;
using NUnit.Framework;
using TestCore;

namespace Tests.ImplTests
{
    public class ClassCreatorWithFuncsTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            classCreator = new ClassCreator(new FuncHelper());
            container = NewMock<IInternalContainer>();
            context = NewMock<IInjectionContext>();
        }

        #endregion

        private ClassCreator classCreator;
        private IInternalContainer container;
        private IInjectionContext context;

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
            public readonly I2 i2;

            public C1(Func<int, I2> createI2)
            {
                i2 = createI2(10);
            }
        }

        // ReSharper restore UnusedMember.Local
        [Test]
        public void TestSimple()
        {
            Func<IInternalContainer, IInjectionContext, object[], object> @delegate =
                classCreator.BuildConstructionDelegate(
                    new ContainerConstructorInfo
                        {
                            ConstructorInfo = typeof (C1).GetConstructor(new[] {typeof (Func<int, I2>)})
                        }, null);
            var c2 = new C2(11, new C3());
            Func<int, I2> func = arg =>
                                     {
                                         Assert.AreEqual(10, arg);
                                         return c2;
                                     };
            container.ExpectBuildCreateFunc(context, func);
            var c1 = (C1) @delegate(container, context, new object[0]);
            Assert.AreSame(c2, c1.i2);
        }
    }
}