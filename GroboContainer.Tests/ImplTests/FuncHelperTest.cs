using System;
using System.Linq;
using System.Reflection;

using GroboContainer.Impl.ClassCreation;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class FuncHelperTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            helper = new FuncHelper();
        }

        #endregion

        private delegate T Func<T1, T2, T3, T4, T5, T>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        [Test]
        public void TestGetBuildCreateFuncMethodInfo()
        {
            Type type = typeof(Func<int, long, Guid, string, object>);
            MethodInfo expected =
                typeof(IFuncBuilder).GetMethods().Where(
                    info => info.ReturnType.IsGenericType && info.ReturnType.GetGenericArguments().Length == 5).Single();
            MethodInfo actual = helper.GetBuildCreateFuncMethodInfo(type);
            Assert.AreSame(expected.MakeGenericMethod(type.GetGenericArguments()), actual);
        }

        [Test]
        public void TestGetBuildLazyMethodInfo()
        {
            Type type = typeof(Lazy<object>);
            MethodInfo expected = typeof(IFuncBuilder).GetMethods().Single(info => info.Name.Contains("BuildLazy"));
            MethodInfo actual = helper.GetBuildLazyMethodInfo(type);
            Assert.AreSame(expected.MakeGenericMethod(type.GetGenericArguments()), actual);
        }

        [Test]
        public void TestGetBuildMethodInfo()
        {
            Type type = typeof(Func<object>);
            MethodInfo expected =
                typeof(IFuncBuilder).GetMethods().Where(
                    info => info.Name.Contains("BuildGetFunc")).Single();
            MethodInfo actual = helper.GetBuildGetFuncMethodInfo(type);
            Assert.AreSame(expected.MakeGenericMethod(type.GetGenericArguments()), actual);
        }

        [Test]
        public void TestGetBuildMethodInfoCrash()
        {
            RunMethodWithException<InvalidOperationException>(() =>
                                                              helper.GetBuildCreateFuncMethodInfo(
                                                                  typeof(Func<int, long, Guid, string, int[], object>)),
                                                              "Функции создания с 5 аргументами на поддерживаются");
        }

        [Test]
        public void TestGetCreateMethodInfoCrash()
        {
            RunMethodWithException<InvalidOperationException>(() =>
                                                              helper.GetBuildGetFuncMethodInfo(
                                                                  typeof(Func<int, int[], object>)),
                                                              "Функции получения с 2 аргументами на поддерживаются");
        }

        [Test]
        public void TestIsFuncFalse()
        {
            Assert.IsFalse(helper.IsFunc(typeof(Func<>)));
            Assert.IsFalse(helper.IsFunc(typeof(Func<,>)));
            Assert.IsFalse(helper.IsFunc(typeof(Func<,,>)));
            Assert.IsFalse(helper.IsFunc(typeof(Zzz<int>)));
            Assert.IsFalse(helper.IsFunc(typeof(int)));
            Assert.IsFalse(helper.IsFunc(typeof(Func<int, long, Guid, string, int[], object>)));
        }

        [Test]
        public void TestIsFuncTrue()
        {
            Assert.That(helper.IsFunc(typeof(Func<int>)));
            Assert.That(helper.IsFunc(typeof(Func<int, long>)));
            Assert.That(helper.IsFunc(typeof(Func<int, long, Guid>)));
            Assert.That(helper.IsFunc(typeof(Func<int, long, Guid, string>)));
            Assert.That(helper.IsFunc(typeof(Func<int, long, Guid, string, object>)));
        }

        [Test]
        public void TestIsLazyFalse()
        {
            Assert.IsFalse(helper.IsLazy(typeof(Lazy<>)));
            Assert.IsFalse(helper.IsLazy(typeof(Zzz<int>)));
            Assert.IsFalse(helper.IsLazy(typeof(int)));
            Assert.IsFalse(helper.IsLazy(typeof(Func<int>)));
        }

        [Test]
        public void TestIsLazyTrue()
        {
            Assert.That(helper.IsLazy(typeof(Lazy<int>)));
            Assert.That(helper.IsLazy(typeof(Lazy<object>)));
        }

        private FuncHelper helper;

        private class Zzz<T>
        {
        }
    }
}