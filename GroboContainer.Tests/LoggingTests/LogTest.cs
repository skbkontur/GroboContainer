using System;

using GroboContainer.Impl.Logging;

using NUnit.Framework;

namespace GroboContainer.Tests.LoggingTests
{
    public class LogTest : CoreTestBase
    {
        #region Setup/Teardown

        public override void SetUp()
        {
            base.SetUp();
            log = new GroboContainerLog("root");
        }

        #endregion

        [Test]
        public void TestCrash()
        {
            log.BeginGet(typeof(long));
            {
                log.BeginConstruct(typeof(int));
                {
                    log.BeginConstruct(typeof(string));
                    log.Crash();
                }
                log.EndConstruct(typeof(int));
                log.Crash();
            }
            log.EndGet(typeof(long));
            Assert.AreEqual(
                @"Container: 'root'
Get<System.Int64>()
 Constructing<System.Int32>()
  Constructing<System.String>()
",
                log.GetLog());
            //Debug.WriteLine(log.GetLog());
        }

        [Test]
        public void TestSimple()
        {
            log.BeginGetAll(typeof(object));
            {
                log.BeginGet(typeof(Guid));
                log.EndGet(typeof(Guid));
                log.BeginGet(typeof(long));
                {
                    log.BeginConstruct(typeof(int));
                    {
                        log.Reused(typeof(string));
                    }
                    log.EndConstruct(typeof(int));
                }
                log.EndGet(typeof(long));
            }
            log.EndGetAll(typeof(long));
            //Debug.WriteLine(log.GetLog());
            Assert.AreEqual(
                @"Container: 'root'
GetAll<System.Object>()
 Get<System.Guid>()
 EndGet<System.Guid>()
 Get<System.Int64>()
  Constructing<System.Int32>()
   Reused<System.String>()
  Constructed<System.Int32>()
 EndGet<System.Int64>()
EndGetAll<System.Int64>()
",
                log.GetLog());
        }

        private IGroboContainerLog log;
    }
}