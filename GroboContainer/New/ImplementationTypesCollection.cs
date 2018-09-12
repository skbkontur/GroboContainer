using System;
using System.Collections.Generic;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.New
{
    public class ImplementationTypesCollection : IImplementationTypesCollection
    {
        public ImplementationTypesCollection(ITypeSource typeSource, ITypesHelper typesHelper)
        {
            this.typeSource = typeSource;
            this.typesHelper = typesHelper;
        }

        #region IImplementationTypesCollection Members

        public Type[] GetImplementationTypes(Type abstractionType)
        {
            var implementationTypes = new List<Type>();
            bool added = false;
            foreach (Type type in typeSource.GetTypesToScan())
                added |= TryAdd(type, abstractionType, implementationTypes);
            if (!added)
                TryAdd(ToDefinition(abstractionType), abstractionType, implementationTypes);
            return implementationTypes.ToArray();
        }

        #endregion

        private static Type ToDefinition(Type abstractionType)
        {
            if (abstractionType.IsGenericType)
                return abstractionType.GetGenericTypeDefinition();
            return abstractionType;
        }

        private bool TryAdd(Type candidate, Type abstractionType,
                            ICollection<Type> implementationTypes)
        {
            if (typesHelper.IsIgnoredImplementation(candidate))
                return false;
            Type implementation = typesHelper.TryGetImplementation(abstractionType, candidate, GetImplementationTypes);
            if (implementation != null)
                implementationTypes.Add(implementation);
            return implementation == abstractionType;
        }

        private readonly ITypeSource typeSource;
        private readonly ITypesHelper typesHelper;
    }
}