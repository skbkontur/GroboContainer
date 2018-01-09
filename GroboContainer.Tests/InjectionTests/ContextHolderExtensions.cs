using GroboContainer.Impl;
using GroboContainer.Impl.Contexts;
using GroboContainer.Impl.Injection;

using NMock2;

namespace GroboContainer.Tests.InjectionTests
{
    public static class ContextHolderExtensions
    {
        public static void ExpectKillContext(this IContextHolder mock)
        {
            Expect.Once.On(mock)
                .Method("KillContext")
                .WithNoArguments();
        }

        public static void ExpectGetContext(this IContextHolder mock, IInternalContainer worker,
                                            IInjectionContext result)
        {
            Expect.Once.On(mock)
                .Method("GetContext")
                .With(worker)
                .Will(Return.Value(result));
        }
    }
}