using System;
using NUnit.Framework;

namespace Tests.TypesHelperTests
{
    public class TypesHelperManyGenericsAbstractTest : TypesHelperGenericTestBase
    {
        private abstract class I1<T1, T2>
        {
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

        private abstract class I2Base<T1, T2, T3>
        {
        }

        private abstract class I2Impl<T1, T2> : I2Base<string, T2, T1>
        {
        }

        private abstract class I3 : I2Impl<Guid, int>
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


        private abstract class Ix1<T>
        {
        }

        private abstract class Ix2<T, T1>
        {
        }

        private class X1<T> : Ix1<Ix2<long, T>>
        {
        }

        [Test]
        public void TestBaseAbstractClass()
        {
            CheckTrue<I2Base<string, int, Guid>, C5>(typeof (C5));
            CheckTrue<I2Base<string, int, long>, C4<int, long>>(typeof (C4<,>));
            CheckTrue<I2Base<string, int, long>, C4A<long, int>>(typeof (C4A<,>));
            CheckTrue<I2Base<string, int, long>, C4Derived<int, long>>(typeof (C4Derived<,>));
            CheckFalse<I2Base<int, int, int>>(typeof (C4<,>));
        }

        [Test]
        public void TestDeep()
        {
            CheckTrue<Ix1<Ix2<long, int>>, X1<int>>(typeof (X1<>));
            CheckFalse<Ix1<Ix2<Guid, int>>>(typeof (X1<>));
        }

        [Test]
        public void TestDefinedTypeParameter()
        {
            CheckTrue<I1<int, long>, C2<int>>(typeof (C2<>));
            CheckFalse<I1<int, string>>(typeof (C2<>));
            CheckFalse<I1<string, int>>(typeof (C3<,>));
        }

        [Test]
        public void TestSimple()
        {
            CheckTrue<I1<int, long>, C1<long, int>>(typeof (C1<,>));
            CheckTrue<I1<int, int>, C1<int, int>>(typeof (C1<,>));
        }
    }
}