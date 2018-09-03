using System;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests
{
    public class TypesHelperManyGenericsTest : TypesHelperGenericTestBase
    {
        private interface I1<T1, T2>
        {
        }

        private interface I2Base<T1, T2, T3>
        {
        }

        private interface I2Impl<T1, T2> : I2Base<string, T2, T1>
        {
        }

        private interface I3 : I2Impl<Guid, int>
        {
        }

        private interface IX1<T>
        {
        }

        private interface IX2<T, T1>
        {
        }

        [Test]
        public void TestBaseInterface()
        {
            CheckTrue<I2Base<string, int, long>, C4<int, long>>(typeof(C4<,>));
            CheckTrue<I2Base<string, int, long>, C4A<long, int>>(typeof(C4A<,>));
            CheckTrue<I2Base<string, int, long>, C4Derived<int, long>>(typeof(C4Derived<,>));
            CheckTrue<I2Base<string, int, Guid>, C5>(typeof(C5));
            CheckFalse<I2Base<int, int, int>>(typeof(C4<,>));
        }

        [Test]
        public void TestDeep()
        {
            CheckTrue<IX1<IX2<long, int>>, X1<int>>(typeof(X1<>));
            CheckFalse<IX1<IX2<Guid, int>>>(typeof(X1<>));
        }

        [Test]
        public void TestDefinedTypeParameter()
        {
            CheckTrue<I1<int, long>, C2<int>>(typeof(C2<>));
            CheckFalse<I1<int, string>>(typeof(C2<>));
            CheckFalse<I1<string, int>>(typeof(C3<,>));
        }

        [Test]
        public void TestSimple()
        {
            CheckTrue<I1<int, long>, C1<long, int>>(typeof(C1<,>));
            CheckTrue<I1<int, int>, C1<int, int>>(typeof(C1<,>));
        }

        private class C1<T1, T2> : I1<T2, T1>
        {
        }

        private class C2<T> : I1<T, long>
        {
        }

        private class C3<T1, T2> : I1<T1, int>
        {
        }

        private class C5 : I3
        {
        }

        private class C4<T1, T2> : I2Impl<T2, T1>
        {
        }

        private class C4A<T1, T2> : I2Impl<T1, T2>
        {
        }

        private class C4Derived<T1, T2> : C4<T1, T2>
        {
        }

        private class X1<T> : IX1<IX2<long, T>>
        {
        }
    }
}