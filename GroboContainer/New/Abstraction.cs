using System;

namespace GroboContainer.New
{
    public class Abstraction : IAbstraction
    {
        public Abstraction(Type[] implementationTypes, IImplementationCache implementationCache)
        {
            implementations = new IImplementation[implementationTypes.Length];
            for (var i = 0; i < implementationTypes.Length; i++)
                implementations[i] = implementationCache.GetOrCreate(implementationTypes[i]);
        }

        public IImplementation[] GetImplementations()
        {
            return implementations;
        }

        private readonly IImplementation[] implementations;
    }
}