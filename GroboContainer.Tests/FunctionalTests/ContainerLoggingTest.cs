using GroboContainer.Core;
using GroboContainer.Impl.Exceptions;

using NUnit.Framework;

namespace GroboContainer.Tests.FunctionalTests
{
    public class ContainerLoggingTest : ContainerTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            lastLog = null;
        }

        private interface I1
        {
        }

        private interface I2
        {
        }

        private interface ICrash1
        {
        }

        private interface ICrash2
        {
        }

        [Test]
        public void TestCrash()
        {
            const string log =
                @"Container: 'root'
Get<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+ICrash1>()
 Constructing<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+Crash1>()
  Get<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+ICrash2>()
   Constructing<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+Crash2>()
    Get<System.Int32>()
";
            RunMethodWithException<ContainerException>(() => container.Get<ICrash1>(), log);
            Assert.AreEqual(log, container.LastConstructionLog);
        }

        [Test]
        public void TestLogContainsLastOperation()
        {
            Assert.AreEqual("<no>", container.LastConstructionLog);
            RunFail<NoConstructorException>(() => container.Get(typeof(int)));
            StringAssert.Contains("Get<System.Int32>()", container.LastConstructionLog);
            container.GetAll<I1>();
            Assert.AreEqual(
                @"Container: 'root'
GetAll<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+I1>()
 Constructing<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+C1>()
 Constructed<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+C1>()
EndGetAll<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+I1>()
",
                container.LastConstructionLog);
        }

        [Test]
        public void TestLogDuringConstruction()
        {
            container.Get<I2>();
            Assert.AreEqual(
                @"Container: 'root'
Get<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+I2>()
 Constructing<GroboContainer.Tests.FunctionalTests.ContainerLoggingTest+C2>()
  Get<GroboContainer.Core.IContainer>()
   Reused<GroboContainer.Core.IContainer>()
  EndGet<GroboContainer.Core.IContainer>()
",
                lastLog);
        }

        private static string lastLog;

        private class C1 : I1
        {
        }

        private class Crash1 : ICrash1
        {
            public Crash1(ICrash2 crash2)
            {
            }
        }

        private class Crash2 : ICrash2
        {
            public Crash2(int i)
            {
            }
        }

        private class C2 : I2
        {
            public C2(IContainer container)
            {
                lastLog = container.LastConstructionLog;
            }
        }
    }
}