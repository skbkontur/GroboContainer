using System;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests
{
    public class TypesHelperGenericParametersTest : TypesHelperGenericTestBase
    {
        private interface I1<T>
        {
        }

        private interface I2<T>
        {
        }

        private interface I3<T>
        {
        }

        private interface I5<T1, T2, T3>
        {
        }

        private class C1<T> : I1<I2<T>>
        {
        }

        private class C3<T> : I3<I2<I1<T>>>
        {
        }

        private interface I4<T1, T2, T3>
        {
        }

        private class C4<T1, T2> : I4<
                                       I1<I2<int>>,
                                       I2<I1<I3<I2<T1>>>>,
                                       I3<I1<I2<T2>>>
                                       >
        {
        }

        private class C5<T1, T2> : I4<
                                       I1<I2<int>>,
                                       I2<I1<I2<I2<T1>>>>,
                                       I3<I1<I2<T2>>>
                                       >
        {
        }

        private class C6<T1, T2> : I4<
                                       I1<I2<int>>,
                                       I2<I1<I2<I2<I1<T1>>>>>,
                                       I3<I1<I2<T2>>>
                                       >
        {
        }

        private class C7<T1, T2> : I4<
                                       I2<int>,
                                       I2<I1<I2<I2<T1>>>>,
                                       I3<I1<I2<T2>>>
                                       >
        {
        }

        private class C4<T> : I5<T, T, int>
        {
        }

        [Test]
        public void TestComplex()
        {
            CheckTrue<I4<I1<I2<int>>, I2<I1<I3<I2<string>>>>, I3<I1<I2<Guid>>>>, C4<string, Guid>>(typeof (C4<,>));
            CheckFalse<I4<I1<I2<long>>, I2<I1<I3<I2<string>>>>, I3<I1<I2<Guid>>>>>(typeof (C4<,>));
            CheckFalse<I4<I1<I2<long>>, I2<I1<I3<I2<string>>>>, I3<I1<I2<Guid>>>>>(typeof (C5<,>));
            CheckFalse<I4<I1<I2<long>>, I2<I1<I3<I2<string>>>>, I3<I1<I2<Guid>>>>>(typeof (C6<,>));
            CheckFalse<I4<I1<I2<long>>, I2<I1<I3<I2<string>>>>, I3<I1<I2<Guid>>>>>(typeof (C7<,>));
        }

        [Test]
        public void TestSeveralEqualsGenerics()
        {
            CheckTrue<I5<long, long, int>, C4<long>>(typeof (C4<>));
            CheckFalse<I5<long, long, long>>(typeof (C4<>));
        }

        [Test]
        public void TestSimple()
        {
            CheckTrue<I1<I2<int>>, C1<int>>(typeof (C1<>));
            CheckTrue<I3<I2<I1<int>>>, C3<int>>(typeof (C3<>));
        }
    }
}