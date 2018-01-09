using System;

using GroboContainer.Impl;
using GroboContainer.Impl.Injection;

using NMock2;

namespace GroboContainer.Tests.ImplTests
{
    public static class InternalContainerExtensions
    {
        public static void ExpectGet<T>(this IInternalContainer mock,
                                        IInjectionContext context, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Get", typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }

        public static void ExpectCreate<T>(this IInternalContainer mock,
                                           IInjectionContext context, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Create", typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }

        public static void ExpectCreate<T1, T>(this IInternalContainer mock,
                                               IInjectionContext context, T1 arg1, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Create", typeof (T1), typeof (T))
                .With(context, arg1)
                .Will(Return.Value(result));
        }

        public static void ExpectCreate<T1, T2, T>(this IInternalContainer mock,
                                                   IInjectionContext context, T1 arg1, T2 arg2, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Create", typeof (T1), typeof (T2), typeof (T))
                .With(context, arg1, arg2)
                .Will(Return.Value(result));
        }

        public static void ExpectCreate<T1, T2, T3, T>(this IInternalContainer mock,
                                                       IInjectionContext context, T1 arg1, T2 arg2, T3 arg3, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Create", typeof (T1), typeof (T2), typeof (T3), typeof (T))
                .With(context, arg1, arg2, arg3)
                .Will(Return.Value(result));
        }

        public static void ExpectCreate<T1, T2, T3, T4, T>(this IInternalContainer mock,
                                                           IInjectionContext context, T1 arg1, T2 arg2, T3 arg3, T4 arg4,
                                                           T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Create", typeof (T1), typeof (T2), typeof (T3), typeof (T4), typeof (T))
                .With(context, arg1, arg2, arg3, arg4)
                .Will(Return.Value(result));
        }

        public static void ExpectGetAndFail<T>(this IInternalContainer mock,
                                               IInjectionContext context, Exception e)
        {
            Expect
                .Once
                .On(mock)
                .Method("Get", typeof (T))
                .With(context)
                .Will(Throw.Exception(e));
        }

        public static void ExpectGetAndFail(this IInternalContainer mock, Type type,
                                            IInjectionContext context, Exception e)
        {
            Expect
                .Once
                .On(mock)
                .Method("Get")
                .With(type, context)
                .Will(Throw.Exception(e));
        }

        public static void ExpectGetAll<T>(this IInternalContainer mock,
                                           IInjectionContext context, T[] result)
        {
            Expect
                .Once
                .On(mock)
                .Method("GetAll", typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }

        public static void ExpectCallDispose(this IInternalContainer mock)
        {
            Expect
                .Once
                .On(mock)
                .Method("CallDispose")
                .WithNoArguments();
        }

        public static void ExpectBuildGetFunc<T>(this IInternalContainer mock,
                                                 IInjectionContext context, Func<T> result)
        {
            Expect
                .Once
                .On(mock)
                .Method("BuildGetFunc", typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }

        public static void ExpectBuildCreateFunc<T>(this IInternalContainer mock,
                                                    IInjectionContext context, Func<T> result)
        {
            Expect
                .Once
                .On(mock)
                .Method("BuildCreateFunc", typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }

        public static void ExpectBuildCreateFunc<T1, T>(this IInternalContainer mock,
                                                        IInjectionContext context, Func<T1, T> result)
        {
            Expect
                .Once
                .On(mock)
                .Method("BuildCreateFunc", typeof (T1), typeof (T))
                .With(context)
                .Will(Return.Value(result));
        }
    }
}