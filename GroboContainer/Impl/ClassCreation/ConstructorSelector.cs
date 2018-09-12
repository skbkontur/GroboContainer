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
        #region IConstructorSelector Members

        public ContainerConstructorInfo GetConstructor(Type source, Type[] parameterTypes)
        {
            ConstructorInfo[] constructors = source.GetConstructors();
            return TryGetAttributedConstructor(source, parameterTypes, constructors)
                   ?? GetNonAttributedConstructor(source, parameterTypes, constructors);
        }

        #endregion

        private static ContainerConstructorInfo GetNonAttributedConstructor(Type source, Type[] parameterTypes,
                                                                            IEnumerable<ConstructorInfo> constructors)
        {
            ContainerConstructorInfo result = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                int[] parametersInfo = CanUseParameters(constructor, parameterTypes);
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

        private static ContainerConstructorInfo TryGetAttributedConstructor(Type source, Type[] parameterTypes,
                                                                            IEnumerable<ConstructorInfo> constructors)
        {
            ContainerConstructorInfo result = null;
            HashSet<Type> parameterTypesSet = GetParameterTypesSet(parameterTypes);
            IEnumerable<ConstructorInfo> attributedConstructors =
                constructors.Where(info => info.IsDefined(typeof(ContainerConstructorAttribute), false));
            bool hasContainerConstructors = false;
            foreach (ConstructorInfo constructor in attributedConstructors)
            {
                hasContainerConstructors = true;
                HashSet<Type> constructorTypesSet = GetConstructorTypesSet(constructor);
                if (parameterTypesSet.SetEquals(constructorTypesSet))
                {
                    int[] parametersInfo = CanUseParameters(constructor, parameterTypes);
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
            ParameterInfo[] parameters = info.GetParameters();
            if (parameters.Length < parameterTypes.Length) return null;
            var permutation = new int[parameters.Length];
            var graph = new BipartiteGraph(parameterTypes.Length, parameters.Length);
            for (int index = 0; index < parameters.Length; index++)
            {
                permutation[index] = -1;
                ParameterInfo parameter = parameters[index];
                if (parameter.IsOut) return null;
                for (int i = 0; i < parameterTypes.Length; i++)
                    if (parameter.ParameterType.IsAssignableFrom(parameterTypes[i]))
                    {
                        graph.AddEdge(i, index);
                    }
            }
            Matching matching;
            if (!matchingBuilder.TryBuild(graph, out matching) || matchingBuilder.HasAnotherMatching(graph, matching))
                return null;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (matching.HasPair(i))
                    permutation[i] = matching.GetPair(i);
            }
            return permutation;
        }

        private static HashSet<Type> GetParameterTypesSet(IEnumerable<Type> parameterTypes)
        {
            var types = new HashSet<Type>();
            foreach (Type type in parameterTypes)
                if (!types.Add(type))
                    throw new ArgumentException(string.Format(
                        "Тип параметра {0} должен присутствовать не более 1 раза", type));
            return types;
        }

        private static HashSet<Type> GetConstructorTypesSet(ConstructorInfo constructor)
        {
            var types = new HashSet<Type>();
            var attribute =
                (ContainerConstructorAttribute)
                constructor.GetCustomAttributes(typeof(ContainerConstructorAttribute), false)[0];
            foreach (Type type in attribute.GetParameterTypes())
                if (!types.Add(type))
                    throw new BadContainerConstructorAttributeException(constructor, type);
            return types;
        }

        private static readonly IMatchingBuilder matchingBuilder = new MatchingBuilder();
    }
}