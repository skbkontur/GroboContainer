using System;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Implementations;
using NMock2;

namespace Tests
{
    public static class AbstractionConfigurationExtensions
    {
        public static void ExpectGetImplementations(this IAbstractionConfiguration mock,
                                                    IImplementationConfiguration[] result)
        {
            Expect
                .Once
                .On(mock)
                .Method("GetImplementations")
                .Will(Return.Value(result));
        }

        public static void ExpectGetImplementations(this IAbstractionConfiguration mock, Exception exception)
        {
            Expect
                .Once
                .On(mock)
                .Method("GetImplementations")
                .Will(Throw.Exception(exception));
        }

        public static void ExpectDisposeInstances(this IAbstractionConfiguration mock)
        {
            Expect
                .Once
                .On(mock)
                .Method("DisposeInstances")
                .WithNoArguments();
        }
    }
}