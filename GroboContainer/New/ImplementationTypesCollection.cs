using System;
using System.Collections.Generic;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.New
{
    public class ImplementationTypesCollection : IImplementationTypesCollection
    {
        private readonly Func<Type, Type[]> getImplementationTypes;

        public ImplementationTypesCollection(ITypeSource typeSource, ITypesHelper typesHelper)
        {
            this.typeSource = typeSource;
            this.typesHelper = typesHelper;

            getImplementationTypes = GetImplementationTypes;
        }

        #region IImplementationTypesCollection Members

        public Type[] GetImplementationTypes(Type abstractionType)
        {
            var implementationTypes = new List<Type>();
            var added = false;
            foreach (var type in typeSource.GetTypesToScan())
                added |= TryAdd(type, abstractionType, implementationTypes);
            if (!added)
                TryAdd(ToDefinition(abstractionType), abstractionType, implementationTypes);
            return implementationTypes.ToArray();
        }

        #endregion

        private static Type ToDefinition(Type abstractionType)
        {
            return abstractionType.IsGenericType ? abstractionType.GetGenericTypeDefinition() : abstractionType;
        }

        private bool TryAdd(Type candidate, Type abstractionType, ICollection<Type> implementationTypes)
        {
            if (typesHelper.IsIgnoredImplementation(candidate))
                return false;
            Type implementation = typesHelper.TryGetImplementation(abstractionType, candidate, getImplementationTypes);
            if (implementation != null)
                implementationTypes.Add(implementation);
            return implementation == abstractionType;
        }

        private readonly ITypeSource typeSource;
        private readonly ITypesHelper typesHelper;
    }
}