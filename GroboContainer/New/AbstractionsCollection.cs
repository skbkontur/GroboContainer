using System;
using System.Collections.Concurrent;

namespace GroboContainer.New
{
    public class AbstractionsCollection : IAbstractionsCollection
    {
        public AbstractionsCollection(IImplementationTypesCollection implementationTypesCollection,
                                      IImplementationCache implementationCache)
        {
            this.implementationTypesCollection = implementationTypesCollection;
            this.implementationCache = implementationCache;
            valueFactory = ValueFactory;
        }

        public IAbstraction Get(Type abstractionType)
        {
            return abstractions.GetOrAdd(abstractionType, valueFactory);
        }

        private IAbstraction ValueFactory(Type abstractionType)
        {
            //NOTE executed outside any lock
            return new Abstraction(implementationTypesCollection.GetImplementationTypes(abstractionType), implementationCache);
        }

        private readonly ConcurrentDictionary<Type, IAbstraction> abstractions = new ConcurrentDictionary<Type, IAbstraction>();
        private readonly IImplementationCache implementationCache;
        private readonly IImplementationTypesCollection implementationTypesCollection;
        private readonly Func<Type, IAbstraction> valueFactory;
    }
}