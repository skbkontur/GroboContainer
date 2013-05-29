using System;
using System.Threading;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;
using NUnit.Framework;
using TestCore;
using Tests.ImplTests;

namespace Tests.InjectionTests
{
    public class InjectionContextTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            internalContainer = NewMock<IInternalContainer>();
            log = NewMock<ILog>();
            holder = null;
            injectionContext = new InjectionContext(internalContainer, log, GetHolder);
        }

        #endregion

        private IContextHolder GetHolder(IInjectionContext context)
        {
            Assert.AreSame(injectionContext,
                           context);
            Assert.IsNull(holder,
                          "duplicate call");
            holder = NewMock<IContextHolder>();
            return holder;
        }

        private IInternalContainer internalContainer;
        private ILog log;
        private InjectionContext injectionContext;
        private IContextHolder holder;

        [Test]
        public void TestCycle()
        {
            Type type = typeof (int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);

            log.ExpectBeginConstruct(typeof (long));
            injectionContext.BeginConstruct(typeof (long));
            log.ExpectGetLog("zzz");
            log.ExpectBeginConstruct(type);
            RunMethodWithException<CyclicDependencyException>(() =>
                                                              injectionContext.BeginConstruct(type), "zzz");
        }

        [Test]
        public void TestGetContainers()
        {
            Assert.IsNull(holder);
            IContainer returnedContainer = injectionContext.Container;
            IContextHolder contextHolder = holder;
            Assert.IsNotNull(contextHolder);

            Assert.IsInstanceOfType(typeof (Container), returnedContainer);
            ((Container) returnedContainer).AssertEqualsTo(new Container(internalContainer, holder, log));

            Assert.AreSame(returnedContainer, injectionContext.Container);
            Assert.AreSame(contextHolder, holder);
        }

        [Test]
        public void TestKillNoHolder()
        {
            Type type = typeof (int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);
            log.ExpectEndConstruct(type);
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestKillWithContainer()
        {
            Type type = typeof (int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);
            injectionContext.Container.ToString(); //container created
            log.ExpectEndConstruct(type);
            holder.ExpectKillContext();
            injectionContext.EndConstruct(type);
        }

        [Test]
        public void TestLogging()
        {
            Type type = typeof (int);
            log.ExpectBeginConstruct(type);
            injectionContext.BeginConstruct(type);

            log.ExpectEndConstruct(type);
            injectionContext.EndConstruct(type);

            log.ExpectBeginCreate(type);
            injectionContext.BeginCreate(type);

            log.ExpectEndCreate(type);
            injectionContext.EndCreate(type);

            log.ExpectCrash();
            injectionContext.Crash();

            Type type1 = typeof (long);
            log.ExpectBeginGet(type1);
            injectionContext.BeginGet(type1);

            log.ExpectEndGet(type1);
            injectionContext.EndGet(type1);

            log.ExpectBeginGetAll(type1);
            injectionContext.BeginGetAll(type1);

            log.ExpectEndGetAll(type1);
            injectionContext.EndGetAll(type1);

            log.ExpectReused(type1);
            injectionContext.Reused(type1);
        }

        [Test]
        public void TestRealConstructor()
        {
            var container = NewMock<IInternalContainer>();
            injectionContext = new InjectionContext(container);
            Assert.IsInstanceOfType(typeof (Container), injectionContext.Container);
            ((Container) injectionContext.Container).AssertEqualsTo(new Container(container,
                                                                                  new ContextHolder(injectionContext,
                                                                                                    Thread.CurrentThread
                                                                                                        .ManagedThreadId),
                                                                                  new Log()));
        }

        [Test]
        public void TestStupid()
        {
            Assert.AreSame(internalContainer, injectionContext.InternalContainer);
            Assert.AreSame(log, injectionContext.GetLog());
        }
    }
}