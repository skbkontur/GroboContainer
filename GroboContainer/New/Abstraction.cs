using System;

namespace GroboContainer.New
{
    public class Abstraction : IAbstraction
    {
        private readonly IImplementation[] implementations;

        public Abstraction(Type[] implementationTypes, IImplementationCache implementationCache)
        {
            implementations = new IImplementation[implementationTypes.Length];
            for (int i = 0; i < implementationTypes.Length; i++)
                implementations[i] = implementationCache.GetOrCreate(implementationTypes[i]);
        }

        #region IAbstraction Members

        public IImplementation[] GetImplementations()
        {
            return implementations;
        }

        #endregion
    }
}