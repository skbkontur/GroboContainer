using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GroboContainer.Impl.Exceptions;
using GroboContainer.Infection;

namespace GroboContainer.Impl.Implementations
{
    public class TypesHelper : ITypesHelper
    {
        public Type TryGetImplementation(Type abstractionType, Type candidate)
        {
            return TryGetImplementation(abstractionType, candidate, null);
        }

        public bool IsIgnoredImplementation(ICustomAttributeProvider provider)
        {
            return provider.IsDefined(typeof(IgnoredImplementationAttribute), false);
        }

        public bool IsIgnoredAbstraction(ICustomAttributeProvider provider)
        {
            return provider.IsDefined(typeof(IgnoredAbstractionAttribute), false);
        }

        public Type TryGetImplementation(Type abstractionType, Type candidate, Func<Type, IEnumerable<Type>> getImplementations)
        {
            if (candidate.IsAbstract || candidate.IsInterface)
                return null;
            if (!candidate.ContainsGenericParameters)
            {
                if (abstractionType.IsAbstract || abstractionType.IsInterface)
                    return abstractionType.IsAssignableFrom(candidate) ? candidate : null;
                return abstractionType == candidate ? candidate : null;
            }

            var candidateArguments = candidate.GetGenericArguments();
            var argumentsCount = candidateArguments.Length;
            var candidateInterfaces = abstractionType.IsInterface
                                          ? candidate.GetInterfaces()
                                          : (abstractionType.IsAbstract ? candidate.ParentsOrSelf() : Enumerable.Repeat(candidate, 1));

            foreach (var candidateInterface in candidateInterfaces)
            {
                var arguments = new Type[argumentsCount];
                if (candidateInterface.MatchWith(abstractionType, arguments))
                {
                    while (arguments.Any(arg => arg == null))
                        if (!MatchFromGenericConstraints(candidate, arguments, candidateArguments, getImplementations))
                            break;
                    if (arguments.Any(arg => arg == null))
                        continue;
                    Type genericType;
                    try
                    {
                        genericType = candidate.MakeGenericType(arguments);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    return genericType;
                }
            }
            return null;
        }

        private bool MatchFromGenericConstraints(Type candidate, Type[] arguments, Type[] candidateArguments, Func<Type, IEnumerable<Type>> getImplementations)
        {
            var resolvedSomething = false;
            for (var i = 0; i < arguments.Length; ++i)
            {
                if (arguments[i] != null)
                    continue;
                var constraints = candidateArguments[i].GetGenericParameterConstraints();
                HashSet<Type> possibleImplementations = null;
                foreach (var constraint in constraints)
                {
                    var constraintResolved = SubstituteResolvedParameters(arguments, candidateArguments, constraint);
                    if (constraintResolved == null)
                        continue;
                    if (getImplementations == null)
                        throw new ArgumentException("Function for type resolution is not specified.");
                    var implementations = getImplementations(constraintResolved);
                    if (possibleImplementations == null)
                        possibleImplementations = new HashSet<Type>(implementations);
                    else
                        possibleImplementations.IntersectWith(implementations);
                }
                if (possibleImplementations == null)
                    continue;

                var validatedImplementations = possibleImplementations
                    .Where(impl => ValidateGenericParameterAttributes(candidateArguments[i], impl))
                    .ToArray();

                if (validatedImplementations.Length == 0)
                    continue;

                if (validatedImplementations.Length == 1)
                {
                    arguments[i] = validatedImplementations[0];
                    resolvedSomething = true;
                    continue;
                }

                throw new ManyGenericSubstitutionsException(candidate, candidateArguments[i], validatedImplementations);
            }
            return resolvedSomething;
        }

        private Type SubstituteResolvedParameters(Type[] resolvedArguments, Type[] candidateArguments, Type constraint)
        {
            if (constraint.IsGenericParameter)
                return resolvedArguments
                    .Where((arg, i) => arg != null && candidateArguments[i] == constraint)
                    .FirstOrDefault();
            if (!constraint.IsGenericType)
                return constraint;
            var genericArgumentTypes = new Type[constraint.GetGenericArguments().Length];
            var genericArguments = constraint.GetGenericArguments();
            for (var i = 0; i < genericArguments.Length; ++i)
                genericArgumentTypes[i] = SubstituteResolvedParameters(resolvedArguments, candidateArguments, genericArguments[i]);
            if (genericArgumentTypes.All(t => t != null))
                return constraint.GetGenericTypeDefinition().MakeGenericType(genericArgumentTypes);
            return null;
        }

        private static bool ValidateGenericParameterAttributes(Type candidateArgument, Type argument)
        {
            var constraints = candidateArgument.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
            if (constraints == GenericParameterAttributes.None)
                return true;

            if (constraints.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                if (!argument.IsClass)
                    return false;
            }

            if (constraints.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                if (!argument.IsValueType)
                    return false;
            }

            if (constraints.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                if (!argument.IsValueType)
                {
                    var defaultConstructor = argument.GetConstructor(Type.EmptyTypes);
                    if (defaultConstructor == null)
                        return false;
                }
            }

            return true;
        }
    }
}