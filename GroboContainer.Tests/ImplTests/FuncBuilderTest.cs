using System;

using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Injection;

using NUnit.Framework;

namespace GroboContainer.Tests.ImplTests
{
    public class FuncBuilderTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            funcBuilder = new FuncBuilder();
            context = NewMock<IInjectionContext>();
            container = NewMock<IContainerForFuncBuilder>();
        }

        #endregion

        [Test]
        public void TestGet()
        {
            Func<int> func = funcBuilder.BuildGetFunc<int>(context);
            context.ExpectGetContainerForFunc(container);
            container.ExpectGetForFunc(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestNoArgs()
        {
            Func<int> func = funcBuilder.BuildCreateFunc<int>(context);
            context.ExpectGetContainerForFunc(container);
            container.ExpectCreateForFunc(1);
            Assert.AreEqual(1, func());
        }

        [Test]
        public void TestOneArg()
        {
            Func<string, int> func = funcBuilder.BuildCreateFunc<string, int>(context);
            context.ExpectGetContainerForFunc(container);
            container.ExpectCreateForFunc("q", 1);
            Assert.AreEqual(1, func("q"));
        }

        private IFuncBuilder funcBuilder;
        private IInjectionContext context;
        private IContainerForFuncBuilder container;
    }
}