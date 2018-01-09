using System;
using GroboContainer.Core;
using GroboContainer.Impl;
using GroboContainer.Impl.Injection;
using GroboContainer.Impl.Logging;
using NMock2;

namespace Tests.ImplTests
{
    public static class InjectionContextExtenstions
    {
        public static void ExpectGetInternalContainer(this IInjectionContext mock, IInternalContainer result)
        {
            Expect.Once.On(mock)
                .GetProperty("InternalContainer")
                .Will(Return.Value(result));
        }

        public static void ExpectGetContainer(this IInjectionContext mock, IContainer result)
        {
            Expect.Once.On(mock)
                .GetProperty("Container")
                .Will(Return.Value(result));
        }

        public static void ExpectGetContainerForFunc(this IInjectionContext mock, IContainerForFuncBuilder result)
        {
            Expect.Once.On(mock)
                .GetProperty("ContainerForFunc")
                .Will(Return.Value(result));
        }

        public static void ExpectGetLog(this IInjectionContext mock, ILog result)
        {
            Expect.Once.On(mock)
                .Method("GetLog")
                .WithNoArguments()
                .Will(Return.Value(result));
        }

        public static void ExpectBeginConstruct(this IInjectionContext mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("BeginConstruct")
                .With(classType);
        }

        public static void ExpectCrash(this IInjectionContext mock)
        {
            Expect.Once.On(mock)
                .Method("Crash")
                .WithNoArguments();
        }

        public static void ExpectEndConstruct(this IInjectionContext mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("EndConstruct")
                .With(classType);
        }

        public static void ExpectReused(this IInjectionContext mock, Type classType)
        {
            Expect.Once.On(mock)
                .Method("Reused")
                .With(classType);
        }

        public static void ExpectBeginGet(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginGet")
                .With(type);
        }

        public static void ExpectEndGet(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndGet")
                .With(type);
        }

        public static void ExpectBeginCreate(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginCreate")
                .With(type);
        }

        public static void ExpectEndCreate(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndCreate")
                .With(type);
        }

        public static void ExpectBeginGetAll(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("BeginGetAll")
                .With(type);
        }

        public static void ExpectEndGetAll(this IInjectionContext mock, Type type)
        {
            Expect.Once.On(mock)
                .Method("EndGetAll")
                .With(type);
        }
    }
}