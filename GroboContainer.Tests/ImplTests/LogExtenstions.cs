using System;

using GroboContainer.Impl.Logging;

using NMock2;

namespace GroboContainer.Tests.ImplTests
{
    public static class LogExtenstions
    {
        public static void ExpectBeginConstruct(this ILog mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("BeginConstruct")
                .With(classType);
        }

        public static void ExpectGetLog(this ILog mock, string log)
        {
            Expect.Once.On(mock)
                .Method("GetLog")
                .WithNoArguments()
                .Will(Return.Value(log));
        }

        public static void ExpectCrash(this ILog mock)
        {
            Expect.Once.On(mock)
                .Method("Crash")
                .WithNoArguments();
        }

        public static void ExpectEndConstruct(this ILog mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("EndConstruct")
                .With(classType);
        }

        public static void ExpectReused(this ILog mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("Reused")
                .With(classType);
        }

        public static void ExpectBeginGet(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginGet")
                .With(type);
        }

        public static void ExpectEndGet(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndGet")
                .With(type);
        }

        public static void ExpectBeginCreate(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginCreate")
                .With(type);
        }

        public static void ExpectEndCreate(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndCreate")
                .With(type);
        }

        public static void ExpectBeginGetAll(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginGetAll")
                .With(type);
        }

        public static void ExpectEndGetAll(this ILog mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndGetAll")
                .With(type);
        }
    }
}