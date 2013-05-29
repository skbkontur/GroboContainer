using System;
using NUnit.Framework;

namespace Tests.FunctionalTests
{
    public class FactoryFuncTest : ContainerTestBase
    {
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
            public readonly int a;
            public readonly I3 service;

            public C2(int a, I3 service)
            {
                this.a = a;
                this.service = service;
            }

            public C2(uint a, string b)
            {
            }

            public C2(uint a, long c)
            {
            }
        }

        private class C1 : I1
        {
            public readonly Func<int, I2> createI2;
            public readonly I2 i2;

            public C1(Func<int, I2> createI2)
            {
                this.createI2 = createI2;
                i2 = createI2(10);
            }
        }

        private class C1TwoArgs
        {
            public readonly Func<int, bool, C1TwoArgsInner> createI2;

            public C1TwoArgs(Func<int, bool, C1TwoArgsInner> createI2)
            {
                this.createI2 = createI2;
            }

            #region Nested type: C1TwoArgsInner

            public class C1TwoArgsInner
            {
                public readonly int a;
                public readonly bool b;
                public readonly I3 service;


                public C1TwoArgsInner(int a, I3 service, bool b)
                {
                    this.a = a;
                    this.service = service;
                    this.b = b;
                }
            }

            #endregion
        }

        private class C13Args
        {
            public readonly Func<int, bool, string, C13ArgsInner> createI2;

            public C13Args(Func<int, bool, string, C13ArgsInner> createI2)
            {
                this.createI2 = createI2;
            }

            #region Nested type: C13ArgsInner

            public class C13ArgsInner
            {
                public readonly int a;
                public readonly bool b;
                public readonly string s;
                public readonly I3 service;


                public C13ArgsInner(string s, int a, I3 service, bool b)
                {
                    this.s = s;
                    this.a = a;
                    this.service = service;
                    this.b = b;
                }
            }

            #endregion
        }

        private class C4Args
        {
            public readonly Func<int, bool, string, int[], C4ArgsInner> createI2;

            public C4Args(Func<int, bool, string, int[], C4ArgsInner> createI2)
            {
                this.createI2 = createI2;
            }

            #region Nested type: C4ArgsInner

            public class C4ArgsInner
            {
                public readonly int a;
                public readonly bool b;
                public readonly int[] ints;
                public readonly string s;
                public readonly I3 service;


                public C4ArgsInner(string s, int a, I3 service, bool b, int[] ints)
                {
                    this.s = s;
                    this.a = a;
                    this.service = service;
                    this.b = b;
                    this.ints = ints;
                }
            }

            #endregion
        }

        [Test]
        public void Test3Args()
        {
            var obj = container.Get<C13Args>();
            C13Args.C13ArgsInner c2 = obj.createI2(1, true, "q");
            Assert.AreEqual(true, c2.b);
            Assert.AreEqual(1, c2.a);
            Assert.AreEqual("q", c2.s);
        }

        [Test]
        public void Test4Args()
        {
            var obj = container.Get<C4Args>();
            var ints = new[] {1};
            C4Args.C4ArgsInner c2 = obj.createI2(1, true, "q", ints);
            Assert.AreEqual(true, c2.b);
            Assert.AreEqual(1, c2.a);
            Assert.AreEqual("q", c2.s);
            Assert.AreSame(ints, c2.ints);
        }

        [Test]
        public void TestSimple()
        {
            var i1 = (C1) container.Get<I1>();
            var c2 = (C2) i1.i2;
            Assert.AreEqual(10, c2.a);
            Assert.IsInstanceOfType(typeof (C3), c2.service);

            var i1Another = (C1) container.Get<I1>();
            Assert.AreSame(c2, i1Another.i2);
            Assert.AreNotSame(c2, i1Another.createI2(20));
        }

        [Test]
        public void TestTwoArgs()
        {
            var obj = container.Get<C1TwoArgs>();
            C1TwoArgs.C1TwoArgsInner c2 = obj.createI2(1, true);
            Assert.AreEqual(true, c2.b);
            Assert.AreEqual(1, c2.a);
        }
    }
}