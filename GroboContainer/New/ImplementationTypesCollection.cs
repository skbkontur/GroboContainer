using System;
using System.Collections.Generic;
using System.Linq;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.New
{
    public class ImplementationTypesCollection : IImplementationTypesCollection
    {
        public ImplementationTypesCollection(IEnumerable<Type> typesToScan, ITypesHelper typesHelper)
        {
            this.typesHelper = typesHelper;
            this.typesToScan = new HashSet<Type>(typesToScan.Where(x => !typesHelper.IsIgnoredImplementation(x)));

            getImplementationTypes = GetImplementationTypes;
        }

        #region IImplementationTypesCollection Members

        public Type[] GetImplementationTypes(Type abstractionType)
        {
            var implementationTypes = new List<Type>();
            var added = false;
            foreach (var typeToScan in typesToScan)
                added |= TryAdd(typeToScan, abstractionType, implementationTypes);
            if (added)
                return implementationTypes.ToArray();

            var definitionType = ToDefinition(abstractionType);
            if (typesToScan.Contains(definitionType) || !typesHelper.IsIgnoredImplementation(definitionType))
                TryAdd(definitionType, abstractionType, implementationTypes);
            return implementationTypes.ToArray();
        }

        #endregion

        private static Type ToDefinition(Type abstractionType)
        {
            return abstractionType.IsGenericType ? abstractionType.GetGenericTypeDefinition() : abstractionType;
        }

        private bool TryAdd(Type candidate, Type abstractionType, ICollection<Type> implementationTypes)
        {
            var implementation = typesHelper.TryGetImplementation(abstractionType, candidate, getImplementationTypes);
            if (implementation != null)
                implementationTypes.Add(implementation);
            return implementation == abstractionType;
        }

        private readonly Func<Type, Type[]> getImplementationTypes;
        private readonly ITypesHelper typesHelper;
        private readonly HashSet<Type> typesToScan;
    }
}