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

		public static bool TryMatch(Type pattern, Type value, Type[] matched)
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
			if (pattern.IsGenericType)
			{
				if (pattern.GetGenericTypeDefinition() != value.GetGenericTypeDefinition())
					return false;
				var whatArguments = pattern.GetGenericArguments();
				var byArguments = value.GetGenericArguments();
				for (var i = 0; i < whatArguments.Length; i++)
					if (!TryMatch(whatArguments[i], byArguments[i], matched))
						return false;
				return true;
			}
			return pattern == value;
		}
	}
}