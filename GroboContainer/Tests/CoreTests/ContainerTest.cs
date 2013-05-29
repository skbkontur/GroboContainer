using System;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;
using NUnit.Framework;
using TestCore;
using Tests.ImplTests;
using Tests.InjectionTests;

namespace Tests.CoreTests
{
    public class ContainerTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            configuration = NewMock<IContainerConfiguration>();
            internalContainer = NewMock<IInternalContainer>();
            holder = NewMock<IContextHolder>();
            context = NewMock<IInjectionContext>();
            log = NewMock<ILog>();
            container = new Container(internalContainer, holder, log);
        }

        #endregion

        private IContainerConfiguration configuration;
        private Container container;
        private IInternalContainer internalContainer;
        private IContextHolder holder;
        private IInjectionContext context;
        private ILog log;

        [Test]
        public void TestCrashGet()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            var mockException = new MockException();
            internalContainer.ExpectGetAndFail<int>(context, mockException);
            context.ExpectGetLog(log);
            log.ExpectGetLog("zzz");
            RunMethodWithException<ContainerException>(() => container.Get<int>(),
                                                       exception =>
                                                           {
                                                               Assert.AreEqual("zzz", exception.Message);
                                                               Assert.AreSame(mockException, exception.InnerException);
                                                           });
        }

        [Test]
        public void TestCrashObjectGet()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            var mockException = new MockException();
            internalContainer.ExpectGetAndFail(typeof (int), context, mockException);
            context.ExpectGetLog(log);
            log.ExpectGetLog("zzz");
            RunMethodWithException<ContainerException>(() => container.Get(typeof (int)),
                                                       exception =>
                                                           {
                                                               Assert.AreEqual("zzz", exception.Message);
                                                               Assert.AreSame(mockException, exception.InnerException);
                                                           });
        }

        [Test]
        public void TestCreateGeneric()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            internalContainer.ExpectCreate(context, 1);
            Assert.AreEqual(1, container.Create<int>());
            log.ExpectGetLog("zzz");
            Assert.AreEqual("zzz", container.LastConstructionLog);
        }

        [Test]
        public void TestCreateGeneric2Args()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            Guid arg2 = Guid.NewGuid();
            internalContainer.ExpectCreate(context, "s", arg2, 1);
            Assert.AreEqual(1, container.Create<string, Guid, int>("s", arg2));
        }

        [Test]
        public void TestCreateGeneric3Args()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            Guid arg2 = Guid.NewGuid();
            internalContainer.ExpectCreate(context, "s", arg2, true, 1);
            Assert.AreEqual(1, container.Create<string, Guid, bool, int>("s", arg2, true));
        }

        [Test]
        public void TestCreateGeneric4Args()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            Guid arg2 = Guid.NewGuid();
            var ints = new[] {2};
            internalContainer.ExpectCreate(context, "s", arg2, true, ints, 1);
            Assert.AreEqual(1, container.Create<string, Guid, bool, int[], int>("s", arg2, true, ints));
        }

        [Test]
        public void TestCreateGenericOneArg()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            internalContainer.ExpectCreate(context, "s", 1);
            Assert.AreEqual(1, container.Create<string, int>("s"));
        }

        [Test]
        public void TestDefaultConstructor()
        {
            container = new Container(configuration);
            container.AssertEqualsTo(new Container(new InternalContainer(configuration), new NoContextHolder(), log));
        }

        [Test]
        public void TestDispose()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetInternalContainer(internalContainer);
            internalContainer.ExpectCallDispose();
            container.Dispose();
        }

        [Test]
        public void TestNoLog()
        {
            container = new Container(configuration);
            Assert.AreEqual("<no>", container.LastConstructionLog);
        }

        [Test]
        public void TestStupid()
        {
            holder.ExpectGetContext(internalContainer, context);
            context.ExpectGetLog(log);
            internalContainer.ExpectGet(context, 1);
            Assert.AreEqual(1, container.Get<int>());
            log.ExpectGetLog("zzz");
            Assert.AreEqual("zzz", container.LastConstructionLog);
        }
    }
}