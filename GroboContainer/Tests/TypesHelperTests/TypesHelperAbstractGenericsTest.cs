using NUnit.Framework;

namespace Tests.TypesHelperTests
{
    public class TypesHelperAbstractGenericsTest : TypesHelperGenericTestBase
    {
        private abstract class AC1<T>
        {
        }

        private abstract class LeftClass<T>
        {
        }

        private class C1<T> : AC1<T>
        {
        }

        private class C2<T> : C1<T>
        {
        }

        [Test]
        public void TestConcreteClassDescendantsNotScanned()
        {
            CheckFalse<C1<int>>(typeof (C2<>));
        }

        [Test]
        public void TestDeepBase()
        {
            CheckTrue<AC1<int>, C2<int>>(typeof (C2<>));
            CheckFalse<LeftClass<int>>(typeof (C2<>));
        }

        [Test]
        public void TestOneLevelBase()
        {
            CheckTrue<AC1<int>, C1<int>>(typeof (C1<>));
        }

        [Test]
        public void TestSimple()
        {
            CheckTrue<C1<int>, C1<int>>(typeof (C1<>));
        }
    }
}