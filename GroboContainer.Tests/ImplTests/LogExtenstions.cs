using System;

using GroboContainer.Impl.Logging;

using NMock2;

namespace GroboContainer.Tests.ImplTests
{
    public static class LogExtenstions
    {
        public static void ExpectBeginConstruct(this IGroboContainerLog mock, Type classType)
        {
            Expect.Once.On(mock)
                  .Method("BeginConstruct")
                  .With(classType);
        }

        public static void ExpectGetLog(this IGroboContainerLog mock, string log)
        {
            Expect.Once.On(mock)
                  .Method("GetLog")
                  .WithNoArguments()
                  .Will(Return.Value(log));
        }

        public static void ExpectCrash(this IGroboContainerLog mock)
        {
            Expect.Once.On(mock)
                  .Method("Crash")
                  .WithNoArguments();
        }

        public static void ExpectEndConstruct(this IGroboContainerLog mock, Type classType)
        {
            Expect.Once.On(mock)
                  .Method("EndConstruct")
                  .With(classType);
        }

        public static void ExpectReused(this IGroboContainerLog mock, Type classType)
        {
            Expect.Once.On(mock)
                  .Method("Reused")
                  .With(classType);
        }

        public static void ExpectBeginGet(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("BeginGet")
                  .With(type);
        }

        public static void ExpectEndGet(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("EndGet")
                  .With(type);
        }

        public static void ExpectBeginCreate(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("BeginCreate")
                  .With(type);
        }

        public static void ExpectEndCreate(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("EndCreate")
                  .With(type);
        }

        public static void ExpectBeginGetAll(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("BeginGetAll")
                  .With(type);
        }

        public static void ExpectEndGetAll(this IGroboContainerLog mock, Type type)
        {
            Expect.Once.On(mock)
                  .Method("EndGetAll")
                  .With(type);
        }
    }
}