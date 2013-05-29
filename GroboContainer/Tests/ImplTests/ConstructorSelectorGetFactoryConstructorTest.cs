using System;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Exceptions;
using NUnit.Framework;

namespace Tests.ImplTests
{
    public class ConstructorSelectorGetFactoryConstructorTest : ConstructorSelectorTestBase
    {
        private interface I1
        {
        }

        private interface I2
        {
        }

        private class Base
        {
        }

        private class Impl : Base
        {
        }

        private class ImplImpl : Impl
        {
        }

        private class C1
        {
            public C1(int i, I1 i1)
            {
            }

            public C1(I1 i1, Base aBase, I2 i2)
            {
            }
        }

        private class C2
        {
            public C2(int a, I2 i2, string b)
            {
            }

            public C2(Guid g1, Guid g2)
            {
            }

            public C2(Guid? g1, Guid g2)
            {
            }

            public C2(out long l)
            {
                l = 1;
            }

            public C2(ref byte b)
            {
            }

            public C2(Base b, Impl i, Enum e, I1 i1)
            {
            }
        }

        [Test]
        public void TestAmbigous()
        {
            RunMethodWithException<AmbiguousConstructorException>(
                () =>
                constructorSelector.GetConstructor(typeof (C1), new[] {typeof (I1)}));
        }

        [Test]
        public void TestParentAndChild()
        {
            ContainerConstructorInfo constructor = constructorSelector.GetConstructor(typeof(C2),
                                                                                      new[] { typeof(Guid), typeof(Guid?) });
            CheckConstructor<C2>(new[] { typeof(Guid?), typeof(Guid) }, constructor, new[] { 1, 0 });
        }

        [Test]
        public void TestAssignableFrom()
        {
            ContainerConstructorInfo constructor = constructorSelector.GetConstructor(typeof (C1),
                                                                                      new[] {typeof (Impl)});
            CheckConstructor<C1>(new[] {typeof (I1), typeof (Base), typeof (I2)}, constructor, new[] {-1, 0, -1});
        }

        [Test]
        public void TestConstructorWithEqualTypesIgnored()
        {
            RunMethodWithException<NoConstructorException>(
                () =>
                constructorSelector.GetConstructor(typeof (C2), new[] {typeof (Guid)}));
        }

        [Test]
        public void TestConstructorWithOutTypesIgnored()
        {
            RunMethodWithException<NoConstructorException>(
                () =>
                constructorSelector.GetConstructor(typeof (C2), new[] {typeof (long)}));
        }

        [Test]
        public void TestConstructorWithrefTypesIgnored()
        {
            RunMethodWithException<NoConstructorException>(
                () =>
                constructorSelector.GetConstructor(typeof (C2), new[] {typeof (byte)}));
        }

        [Test]
        public void TestEqualParameterTypes()
        {
            RunMethodWithException<ArgumentException>(
                () =>
                constructorSelector.GetConstructor(typeof (C2), new[] {typeof (int), typeof (int)}));
        }

        [Test]
        public void TestManyTypes()
        {
            ContainerConstructorInfo constructor = constructorSelector.GetConstructor(typeof (C2),
                                                                                      new[]
                                                                                          {
                                                                                              typeof (string),
                                                                                              typeof (int)
                                                                                          });
            CheckConstructor<C2>(new[] {typeof (int), typeof (I2), typeof (string)}, constructor, new[] {1, -1, 0});
        }

        [Test]
        public void TestMegaBug()
        {
            RunMethodWithException<NoConstructorException>(
                () =>
                constructorSelector.GetConstructor(typeof (C2), new[] {typeof (ImplImpl), typeof (sbyte)}));
        }

        [Test]
        public void TestNo()
        {
            RunMethodWithException<NoConstructorException>(() =>
                                                           constructorSelector.GetConstructor(typeof (C1),
                                                                                              new[]
                                                                                                  {
                                                                                                      typeof (
                                                                                                          short)
                                                                                                  }));
        }

        [Test]
        public void TestSimple()
        {
            ContainerConstructorInfo constructor = constructorSelector.GetConstructor(typeof (C1),
                                                                                      new[] {typeof (int)});
            CheckConstructor<C1>(new[] {typeof (int), typeof (I1)}, constructor, new[] {0, -1});
        }
    }
}