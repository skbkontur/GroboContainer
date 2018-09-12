using System;
using System.Collections.Generic;

namespace GroboContainer.Impl.Implementations
{
    public static class ReflectionHelpers
    {
        public static IEnumerable<Type> ParentsOrSelf(this Type type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool MatchWith(this Type pattern, Type value, Type[] matched)
        {
            if (pattern.IsGenericParameter)
            {
                var position = pattern.GenericParameterPosition;
                if (matched[position] != null && matched[position] != value)
                    return false;
                matched[position] = value;
                return true;
            }
            if (pattern.IsGenericType ^ value.IsGenericType)
                return false;
            if (pattern.IsArray && value.IsArray)
            {
                var type = pattern.GetElementType();
                var value_type = value.GetElementType();
                if (!MatchWith(type, value_type, matched))
                    return false;
                return true;
            }
            if (!pattern.IsGenericType)
                return pattern == value;
            if (pattern.GetGenericTypeDefinition() != value.GetGenericTypeDefinition())
                return false;
            var patternArguments = pattern.GetGenericArguments();
            var valueArguments = value.GetGenericArguments();
            for (var i = 0; i < patternArguments.Length; i++)
                if (!patternArguments[i].MatchWith(valueArguments[i], matched))
                    return false;
            return true;
        }
    }
}