using System.Reflection;

using NMock2;

namespace GroboContainer.Tests
{
    public static class CustomAttributeProviderExtensions
    {
        public static void ExpectIsDefined<T>(this ICustomAttributeProvider mock, bool result)
        {
            Expect.Once.On(mock)
                .Method("IsDefined")
                .With(typeof (T), false)
                .Will(Return.Value(result));
        }
    }
}