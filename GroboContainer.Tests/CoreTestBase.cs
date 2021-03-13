using System;
using System.Threading;

using Moq;

using NUnit.Framework;

namespace GroboContainer.Tests
{
    public abstract class CoreTestBase
    {
        [SetUp]
        public virtual void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }

        protected Mock<T> GetMock<T>() where T : class
        {
            return mockRepository.Create<T>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                mockRepository.VerifyAll();
                mockRepository.VerifyNoOtherCalls();
            }
            finally
            {
                FieldsCleanerCache.Clean(this);
            }
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

        private MockRepository mockRepository;
    }
}