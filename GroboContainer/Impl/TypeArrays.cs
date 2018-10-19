using System;

namespace GroboContainer.Impl
{
    internal static class TypeArrays<T>
    {
        public static readonly Type[] Instance = {typeof(T)};
    }

    internal static class TypeArrays<T1, T2>
    {
        public static readonly Type[] Instance = {typeof(T1), typeof(T2)};
    }

    internal static class TypeArrays<T1, T2, T3>
    {
        public static readonly Type[] Instance = {typeof(T1), typeof(T2), typeof(T3)};
    }

    internal static class TypeArrays<T1, T2, T3, T4>
    {
        public static readonly Type[] Instance = {typeof(T1), typeof(T2), typeof(T3), typeof(T4)};
    }
}