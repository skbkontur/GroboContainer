using System;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Infection;
using NUnit.Framework;

namespace Tests.ImplTests
{
    public class ConstructorSelectorWithContainerConstructorTest : ConstructorSelectorTestBase
    {
        private class C1
        {
            [ContainerConstructor]
            public C1(int i)
            {
            }

            public C1()
            {
            }
        }

        private class C3
        {
            [ContainerConstructor(typeof (int), typeof (int))]
            public C3(int i, string s)
            {
            }
        }

        private class C4
        {
            [ContainerConstructor(typeof (int))]
            public C4(string s)
            {
            }
        }

        private class C2
        {
            [ContainerConstructor(typeof (int))]
            public C2(int i, string s)
            {
            }

            [ContainerConstructor(typeof (long))]
            public C2(int i, long z)
            {
            }
        }

        private class C5
        {
            [ContainerConstructor(typeof (int))]
            public C5(int i, string s)
            {
            }

            [ContainerConstructor(typeof (int))]
            public C5(int i, long z)
            {
            }
        }

        [Test]
        public void TestAmbigous()
        {
            RunMethodWithException<AmbiguousConstructorException>(() =>
                                                                  constructorSelector.GetConstructor(
                                                                      typeof (C5),
                                                                      new[] {typeof (int)}));
        }


        [Test]
        public void TestAttrDoesNotFitToConstructor()
        {
            RunMethodWithException<BadContainerConstructorAttributeException>(() =>
                                                                              constructorSelector.GetConstructor(
                                                                                  typeof (C4),
                                                                                  new[] {typeof (int)}));
        }

        [Test]
        public void TestBadTypesInAttr()
        {
            RunMethodWithException<BadContainerConstructorAttributeException>(() =>
                                                                              constructorSelector.GetConstructor(
                                                                                  typeof (C3),
                                                                                  new[] {typeof (int), typeof (long)}));
        }

        [Test]
        public void TestFindByParameters()
        {
            CheckConstructor<C2>(new[] {typeof (int), typeof (long)},
                                 constructorSelector.GetConstructor(typeof (C2), new[] {typeof (long)}),
                                 new[] {-1, 0});
        }

        [Test]
        public void TestNoParameters()
        {
            CheckConstructor<C1>(new[] {typeof (int)},
                                 constructorSelector.GetConstructor(typeof (C1), Type.EmptyTypes), new[] {-1});
        }

        [Test]
        public void TestNotFound()
        {
            RunMethodWithException<NoConstructorException>(() =>
                                                           constructorSelector.GetConstructor(
                                                               typeof (C1),
                                                               new[] {typeof (int)}));
        }
    }
}