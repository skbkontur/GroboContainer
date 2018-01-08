using System;
using GroboContainer.Impl.Exceptions;
using NUnit.Framework;

namespace Tests
{
    public abstract class TestBase : CoreTestBase
    {
        protected static void RunFail<TE>(Action action) where TE : Exception
        {
            RunFail<TE>(action, "");
        }

        protected static void RunFail<TE>(Action action, string msg) where TE : Exception
        {
            RunMethodWithException<ContainerException>(action,
                                                       exception =>
                                                           {
                                                               Assert.IsInstanceOfType(typeof (TE),
                                                                                       exception.InnerException);
                                                               StringAssert.Contains(msg,
                                                                                     exception.InnerException.Message);
                                                           });
        }
    }
}