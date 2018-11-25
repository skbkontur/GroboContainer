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

        #region IAbstraction Members

        public IImplementation[] GetImplementations()
        {
            return implementations;
        }

        #endregion

        private readonly IImplementation[] implementations;
    }
}