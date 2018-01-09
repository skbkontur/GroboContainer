using System;

using GroboContainer.Impl.Abstractions;

using NMock2;

namespace GroboContainer.Tests
{
    public static class AbstractionConfigurationCollectionExtensions
    {
        public static void ExpectGet(this IAbstractionConfigurationCollection mock, Type abstractionType,
                                     IAbstractionConfiguration result)
        {
            Expect
                .Once
                .On(mock)
                .Method("Get")
                .With(abstractionType)
                .Will(Return.Value(result));
        }

        public static void ExpectGetAll(this IAbstractionConfigurationCollection mock,
                                        IAbstractionConfiguration[] configurations)
        {
            Expect
                .Once
                .On(mock)
                .Method("GetAll")
                .WithNoArguments()
                .Will(Return.Value(configurations));
        }
    }
}