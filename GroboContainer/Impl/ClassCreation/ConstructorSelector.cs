using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using GroboContainer.Algorithms.Builders;
using GroboContainer.Algorithms.DataStructures;
using GroboContainer.Impl.Exceptions;
using GroboContainer.Infection;

namespace GroboContainer.Impl.ClassCreation
{
    public class ConstructorSelector : IConstructorSelector
    {
        public ContainerConstructorInfo GetConstructor(Type source, Type[] parameterTypes)
        {
            var constructors = source.GetConstructors();
            return TryGetAttributedConstructor(source, parameterTypes, constructors)
                   ?? GetNonAttributedConstructor(source, parameterTypes, constructors);
        }

        private static ContainerConstructorInfo GetNonAttributedConstructor(Type source, Type[] parameterTypes, IEnumerable<ConstructorInfo> constructors)
        {
            ContainerConstructorInfo result = null;
            foreach (var constructor in constructors)
            {
                var parametersInfo = CanUseParameters(constructor, parameterTypes);
                if (parametersInfo != null)
                {
                    if (result != null)
                        throw new AmbiguousConstructorException(source);
                    result = new ContainerConstructorInfo
                        {
                            ConstructorInfo = constructor,
                            ParametersInfo = parametersInfo
                        };
                }
            }
            if (result == null)
                throw new NoConstructorException(source);
            return result;
        }

        private static ContainerConstructorInfo TryGetAttributedConstructor(Type source, Type[] parameterTypes, IEnumerable<ConstructorInfo> constructors)
        {
            ContainerConstructorInfo result = null;
            var parameterTypesSet = GetParameterTypesSet(parameterTypes);
            var attributedConstructors =
                constructors.Where(info => info.IsDefined(typeof(ContainerConstructorAttribute), false));
            var hasContainerConstructors = false;
            foreach (var constructor in attributedConstructors)
            {
                hasContainerConstructors = true;
                var constructorTypesSet = GetConstructorTypesSet(constructor);
                if (parameterTypesSet.SetEquals(constructorTypesSet))
                {
                    var parametersInfo = CanUseParameters(constructor, parameterTypes);
                    if (parametersInfo == null)
                        throw new BadContainerConstructorAttributeException(constructor);
                    if (result != null)
                        throw new AmbiguousConstructorException(source);
                    result = new ContainerConstructorInfo
                        {
                            ConstructorInfo = constructor,
                            ParametersInfo = parametersInfo
                        };
                }
            }
            if (hasContainerConstructors && result == null)
                throw new NoConstructorException(source);
            return result;
        }

        private static int[] CanUseParameters(MethodBase info, Type[] parameterTypes)
        {
            var parameters = info.GetParameters();
            if (parameters.Length < parameterTypes.Length) return null;
            var permutation = new int[parameters.Length];
            var graph = new BipartiteGraph(parameterTypes.Length, parameters.Length);
            for (var index = 0; index < parameters.Length; index++)
            {
                permutation[index] = -1;
                var parameter = parameters[index];
                if (parameter.IsOut) return null;
                for (var i = 0; i < parameterTypes.Length; i++)
                    if (parameter.ParameterType.IsAssignableFrom(parameterTypes[i]))
                    {
                        graph.AddEdge(i, index);
                    }
            }
            if (!matchingBuilder.TryBuild(graph, out var matching) || matchingBuilder.HasAnotherMatching(graph, matching))
                return null;

            for (var i = 0; i < parameters.Length; i++)
            {
                if (matching.HasPair(i))
                    permutation[i] = matching.GetPair(i);
            }
            return permutation;
        }

        private static HashSet<Type> GetParameterTypesSet(IEnumerable<Type> parameterTypes)
        {
            var types = new HashSet<Type>();
            foreach (var type in parameterTypes)
            {
                if (!types.Add(type))
                    throw new ArgumentException($"Parameter type {type} must not appear more than once");
            }
            return types;
        }

        private static HashSet<Type> GetConstructorTypesSet(ConstructorInfo constructor)
        {
            var types = new HashSet<Type>();
            var attribute = (ContainerConstructorAttribute)constructor.GetCustomAttributes(typeof(ContainerConstructorAttribute), false)[0];
            foreach (var type in attribute.GetParameterTypes())
            {
                if (!types.Add(type))
                    throw new BadContainerConstructorAttributeException(constructor, type);
            }
            return types;
        }

        private static readonly IMatchingBuilder matchingBuilder = new MatchingBuilder();
    }
}