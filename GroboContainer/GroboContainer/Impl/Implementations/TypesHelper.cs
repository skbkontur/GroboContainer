using System;
using System.Reflection;
using GroboContainer.Infection;

namespace GroboContainer.Impl.Implementations
{
    public class TypesHelper : ITypesHelper
    {
        #region ITypesHelper Members

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

            if (abstractionType.IsInterface)
            {
                Type[] interfaces = candidate.GetInterfaces();
                foreach (Type @interface in interfaces)
                {
                    Type result = TryBuildType(candidate, abstractionType, @interface);
                    if (result != null) return result;
                }
            }
            else
            {
                Type currentType = candidate;
                if (!abstractionType.IsAbstract)
                    return TryBuildType(candidate, abstractionType, currentType);
                while (currentType.BaseType != null)
                {
                    Type implementation = TryBuildType(candidate, abstractionType, currentType);
                    if (implementation != null)
                        return implementation;
                    currentType = currentType.BaseType;
                }
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

        #endregion

        private static bool TryBuildClassArguments(Type abstractionType, Type typeToCheck, Type[] classArguments,
                                                   Type[] typeArguments)
        {
            if (typeToCheck.GetGenericTypeDefinition() != abstractionType.GetGenericTypeDefinition())
                return false;
            Type[] genericArguments = typeToCheck.GetGenericArguments();
            Type[] requiredArguments = abstractionType.GetGenericArguments();
            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i].IsGenericParameter)
                {
                    //TODO если не нашли, то Assert.Bug()
                    for (int index = 0; index < typeArguments.Length; index++)
                        if (typeArguments[index] == genericArguments[i])
                        {
                            classArguments[index] = requiredArguments[i];
                        }
                }
                else
                {
                    if (!genericArguments[i].ContainsGenericParameters)
                    {
                        if (requiredArguments[i] != genericArguments[i]) return false;
                    }
                    else if (
                        !TryBuildClassArguments(requiredArguments[i], genericArguments[i], classArguments,
                                                typeArguments))
                        return false;
                }
            }
            return true;
        }

        private static Type TryBuildType(Type type, Type abstractionType, Type typeToCheck)
        {
            if (!typeToCheck.IsGenericType)
                return null;
            Type[] typeArguments = type.GetGenericArguments(); //ok, все IsGenericParameter = true
            var classArguments = new Type[typeArguments.Length];
            if (TryBuildClassArguments(abstractionType, typeToCheck, classArguments, typeArguments))
            {
                foreach (Type argument in classArguments)
                    if (argument == null)
                        return null;
                return type.GetGenericTypeDefinition().MakeGenericType(classArguments);
            }
            return null;
        }
    }
}