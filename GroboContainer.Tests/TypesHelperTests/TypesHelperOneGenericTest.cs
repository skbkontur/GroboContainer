using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests
{
    public class TypesHelperOneGenericTest : TypesHelperGenericTestBase
    {
        private interface I1
        {
        }

        private abstract class Ca : I1
        {
        }

        private class C1<T> : Ca, I1
        {
        }

        private interface I2<T>
        {
        }

        private class C2<T> : I2<T>
        {
        }

        private class C3<T1, T2> : I2<T1>
        {
        }

        [Test]
        public void TestGenericTypeAndNotGenericInterface()
        {
            CheckFalse<I1>(typeof (C1<>));
            CheckFalse<Ca>(typeof (C1<>));
        }

        [Test]
        public void TestOneArgInterface()
        {
            CheckTrue<I2<int>, C2<int>>(typeof (C2<>));
            CheckFalse<I2<int>>(typeof (C3<,>));
            CheckFalse<I2<int>>(typeof (C1<>));
        }
    }
}