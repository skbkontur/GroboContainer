using System;
using GroboContainer.Impl.Logging;
using NUnit.Framework;
using TestCore;

namespace Tests.LoggingTests
{
    public class ShortLogTest : CoreTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            log = new ShortLog("root");
        }

        private ILog log;

        [Test]
        public void TestCrash()
        {
            log.BeginGet(typeof (long));
            {
                log.BeginConstruct(typeof (int));
                {
                    log.BeginConstruct(typeof (string));
                    log.Crash();
                }
                log.EndConstruct(typeof (int));
                log.Crash();
            }
            log.EndGet(typeof (long));
            Assert.AreEqual(
                @"Container: 'root'
Get<System.Int64>()
 Constructing<System.Int32>()
  Constructing<System.String>()
",
                log.GetLog());
        }

        [Test]
        public void TestCrash2()
        {
            log.BeginGet(typeof (long));
            {
                log.BeginConstruct(typeof (int));
                {
                }
                log.EndConstruct(typeof (int));
                log.BeginConstruct(typeof (string));
                {
                    log.Crash();
                    log.EndConstruct(typeof (string));
                }
            }
            log.EndGet(typeof (long));
            Assert.AreEqual(
                @"Container: 'root'
Get<System.Int64>()
 Constructing<System.String>()
",
                log.GetLog());
            //Debug.WriteLine(log.GetLog());
        }

        [Test]
        public void TestSimple()
        {
            log.BeginGetAll(typeof (object));
            {
                log.BeginGet(typeof (Guid));
                log.EndGet(typeof (Guid));
                log.BeginGet(typeof (long));
                {
                    log.BeginConstruct(typeof (int));
                    {
                        log.Reused(typeof (string));
                    }
                    log.EndConstruct(typeof (int));
                }
            }
            //Debug.WriteLine(log.GetLog());
            Assert.AreEqual(
                @"Container: 'root'
GetAll<System.Object>()
 Get<System.Int64>()
",
                log.GetLog());
        }

        [Test]
        public void TestSimple2()
        {
            log.BeginGetAll(typeof (object));
            {
                log.BeginGet(typeof (Guid));
                log.EndGet(typeof (Guid));
                log.BeginGet(typeof (long));
                {
                    log.BeginConstruct(typeof (int));
                    {
                        log.Reused(typeof (string));
                    }
                    log.EndConstruct(typeof (int));
                }
                log.BeginCreate(typeof (string));
                {
                    log.BeginConstruct(typeof (short));
                }
            }
            //Debug.WriteLine(log.GetLog());
            Assert.AreEqual(
                @"Container: 'root'
GetAll<System.Object>()
 Get<System.Int64>()
  Create<System.String>()
   Constructing<System.Int16>()
",
                log.GetLog());
        }
    }
}