using System;
using System.Threading;

using NMock2;

using NUnit.Framework;

namespace GroboContainer.Tests
{
    public abstract class CoreTestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            mockery = new Mockery();
            mockRepository = new MockRepositoryWrapper();
        }

        protected static T NewMock<T>()
        {
            return mockery.NewMock<T>();
        }

        protected static T GetMock<T>()
        {
            return mockRepository.GetMock<T>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                mockRepository.VerifyAll();
                mockery.Dispose();
            }
            finally
            {
                ClearFields();
            }
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            ClearFields();
        }

        private void ClearFields()
        {
            FieldsCleanerCache.Clean(this);
        }

        protected static void RunMethodWithException<TE>(Action method) where TE : Exception
        {
            RunMethodWithException(method, (Action<TE>)null);
        }

        protected static void RunMethodWithException<TE>(Action method, string expectedMessageSubstring) where TE : Exception
        {
            Assert.That(!string.IsNullOrEmpty(expectedMessageSubstring));
            RunMethodWithException<TE>(method, e => StringAssert.Contains(expectedMessageSubstring, e.Message));
        }

        protected static void RunMethodWithException<TE>(Action method, Action<TE> exceptionCheckDelegate) where TE : Exception
        {
            if (typeof(TE) == typeof(Exception) || typeof(TE) == typeof(AssertionException))
                Assert.Fail("использование типа {0} запрещено", (object)typeof(TE));
            try
            {
                method();
            }
            catch (TE ex)
            {
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
                exceptionCheckDelegate?.Invoke(ex);
                return;
            }
            Assert.Fail("Method didn't thrown expected exception " + typeof(TE));
        }

        protected static Mockery mockery;
        private static MockRepositoryWrapper mockRepository;
    }
}