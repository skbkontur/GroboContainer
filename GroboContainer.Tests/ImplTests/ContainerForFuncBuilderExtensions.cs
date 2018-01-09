using GroboContainer.Core;
using NMock2;

namespace Tests.ImplTests
{
    public static class ContainerForFuncBuilderExtensions
    {
        public static void ExpectGetForFunc<T>(this IContainerForFuncBuilder mock, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("GetForFunc", typeof (T))
                .With()
                .Will(Return.Value(result));
        }

        public static void ExpectCreateForFunc<T>(this IContainerForFuncBuilder mock, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("CreateForFunc", typeof (T))
                .With()
                .Will(Return.Value(result));
        }

        public static void ExpectCreateForFunc<T1, T>(this IContainerForFuncBuilder mock, T1 arg1, T result)
        {
            Expect
                .Once
                .On(mock)
                .Method("CreateForFunc", typeof (T1), typeof (T))
                .With(arg1)
                .Will(Return.Value(result));
        }
    }
}