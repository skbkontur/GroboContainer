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

			var argumentsCount = candidate.GetGenericArguments().Length;
			var candidateInterfaces = abstractionType.IsInterface
				? candidate.GetInterfaces()
				: (abstractionType.IsAbstract ? candidate.ParentsOrSelf() : Enumerable.Repeat(candidate, 1));
			foreach (var candidateInterface in candidateInterfaces)
			{
				var arguments = new Type[argumentsCount];
				if (candidateInterface.MatchWith(abstractionType, arguments) && arguments.All(x => x != null))
					return candidate.MakeGenericType(arguments);
			}
			return null;
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