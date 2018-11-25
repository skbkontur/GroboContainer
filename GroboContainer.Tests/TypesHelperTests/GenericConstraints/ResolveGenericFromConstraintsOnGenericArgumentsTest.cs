using System;

using GroboContainer.Impl.Exceptions;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests.GenericConstraints
{
    public class ResolveGenericFromConstraintsOnGenericArgumentsTest : TypesHelperGenericTestBase
    {
        private interface I<T>
        {
        }

        private interface K<T1, T2>
        {
        }

        private interface J<T>
        {
        }

        private interface M<T>
        {
        }

        private interface IC
        {
        }

        [Test]
        public void TestCannotResolveWhenManyTypesMatchGenericConstraint()
        {
            Assert.Throws<ManyGenericSubstitutionsException>(() => helpers.TryGetImplementation(typeof(IC), typeof(C3<>), (Func<Type, Type[]>)(t => new[] {typeof(C3Arg), typeof(AnotherC3Arg)})));
        }

        [Test]
        public void TestDoNotResolveCyclicGenerics()
        {
            CheckFalse<IC>(typeof(CyclicGenerics<,>));
        }

        [Test]
        public void TestDoNotResolveGenericFromSelf()
        {
            CheckFalse<IC>(typeof(GenericFromSelf<>));
        }

        [Test]
        public void TestResolveGenericConstraintOnGenericArgument()
        {
            CheckTrue<I<IC>, C2<IC, C1>>(typeof(C2<,>), t =>
                {
                    if (t == typeof(IC))
                        return new[] {typeof(C1)};
                    return new Type[0];
                });
        }

        [Test]
        public void TestResolveGenericConstraintOnGenericFromClassAndGenericArgument()
        {
            CheckTrue<I<X>, D<X, Z>>(typeof(D<,>), t =>
                {
                    if (t == typeof(K<X, X>))
                        return new[] {typeof(Z)};
                    return new Type[0];
                });
        }

        [Test]
        public void TestResolveGenericConstraintOnGenericType()
        {
            CheckTrue<I<X>, C<X, Y>>(typeof(C<,>), t =>
                {
                    if (t == typeof(J<X>))
                        return new[] {typeof(Y)};
                    return new Type[0];
                });
        }

        [Test]
        public void TestResolveGenericConstraintStepByStep()
        {
            CheckTrue<I<X>, N<X, H, Y>>(typeof(N<,,>), t =>
                {
                    if (t == typeof(J<X>))
                        return new[] {typeof(Y)};
                    if (t == typeof(K<X, Y>))
                        return new[] {typeof(H)};
                    return new Type[0];
                });
        }

        [Test]
        public void TestResolveNestedGenericConstraint()
        {
            CheckTrue<I<X>, B<X, A>>(typeof(B<,>), t =>
                {
                    if (t == typeof(M<J<X>>))
                        return new[] {typeof(A)};
                    return new Type[0];
                });
        }

        private class X
        {
        }

        private class Z : K<X, X>
        {
        }

        private class D<T1, T2> : I<T1>
            where T2 : K<T1, X>
        {
        }

        private class Y : J<X>
        {
        }

        private class C<T1, T2> : I<T1>
            where T2 : J<T1>
        {
        }

        private class A : M<J<X>>
        {
        }

        private class B<T1, T2> : I<T1>
            where T2 : M<J<T1>>
        {
        }

        private class C1 : IC
        {
        }

        private class C2<T1, T2> : I<T1>
            where T2 : T1
        {
        }

        private class C3<T> : IC where T : C3Arg
        {
        }

        private class C3Arg
        {
        }

        private class AnotherC3Arg : C3Arg
        {
        }

        private class H : K<X, Y>
        {
        }

        private class N<T1, T3, T2> : I<T1>
            where T2 : J<T1>
            where T3 : K<T1, T2>
        {
        }

        private class GenericFromSelf<T> : IC
            where T : GenericFromSelf<T>
        {
        }

        private class Arg : GenericFromSelf<Arg>
        {
        }

        private class CyclicGenerics<T1, T2> : IC
            where T1 : G1<T2>
            where T2 : G1<T1>
        {
        }

        private class G1<T>
        {
        }

        private class X1 : G1<X2>
        {
        }

        private class X2 : G1<X1>
        {
        }
    }
}