using System;
using System.Linq;
using System.Reflection;
using GroboContainer.Infection;

namespace GroboContainer.Impl.Implementations
{
	public class TypesHelper : ITypesHelper
	{
		public Type TryGetImplementation(Type abstractionType, Type candidate)
		{
			if (candidate.IsAbstract || candidate.IsInterface) return null;
			if (!candidate.ContainsGenericParameters)
			{
				if (abstractionType.IsAbstract || abstractionType.IsInterface)
					return abstractionType.IsAssignableFrom(candidate) ? candidate : null;
				return abstractionType == candidate ? candidate : null;
			}

			if (!abstractionType.IsGenericType)
				return null;

			var candidateArguments = candidate.GetGenericArguments();
			var argumentsCount = candidateArguments.Length;
			var candidateInterfaces = abstractionType.IsInterface
				? candidate.GetInterfaces()
				: (abstractionType.IsAbstract ? candidate.ParentsOrSelf() : Enumerable.Repeat(candidate, 1));

			foreach (var candidateInterface in candidateInterfaces)
			{
				var arguments = new Type[argumentsCount];
				if (candidateInterface.MatchWith(abstractionType, arguments) && ValidateGenericArguments(arguments, candidateArguments))
					return candidate.MakeGenericType(arguments);
			}
			return null;
		}

		private static bool ValidateGenericArguments(Type[] arguments, Type[] candidateArguments)
		{
			if (arguments.Any(x => x == null))
				return false;

			for (var i = 0; i < candidateArguments.Length; i++)
			{
				var candidateArgument = candidateArguments[i];
				var argument = arguments[i];

				if (!ValidateGenericParameterAttributes(candidateArgument, argument) ||
					!ValidateGenericParameterInheritance(candidateArgument, argument))
				{
					return false;
				}
			}

			return true;
		}

		private static bool ValidateGenericParameterAttributes(Type candidateArgument, Type argument)
		{
			var constraints = candidateArgument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
			if (constraints == GenericParameterAttributes.None)
			{
				return true;
			}

			if (constraints.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
			{
				if (!argument.IsClass)
				{
					return false;
				}
			}

			if (constraints.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
			{
				if (!argument.IsValueType)
				{
					return false;
				}
			}

			if (constraints.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
			{
				if (!argument.IsValueType)
				{
					var defaultConstructor = argument.GetConstructor(Type.EmptyTypes);
					if (defaultConstructor == null)
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool ValidateGenericParameterInheritance(Type candidateArgument, Type argument)
		{
			var candidateArgumentConstraints = candidateArgument.GetGenericParameterConstraints();
			return candidateArgumentConstraints.All(candidateArgumentConstraint => candidateArgumentConstraint.IsAssignableFrom(argument));
		}

		public bool IsIgnoredImplementation(ICustomAttributeProvider provider)
		{
			return provider.IsDefined(typeof (IgnoredImplementationAttribute), false);
		}

		public bool IsIgnoredAbstraction(ICustomAttributeProvider provider)
		{
			return provider.IsDefined(typeof (IgnoredAbstractionAttribute), false);
		}
	}
}