using System;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class ForbiddenAbstractionConfiguration : IAbstractionConfiguration
    {
        public ForbiddenAbstractionConfiguration(Type abstractionType)
        {
            this.abstractionType = abstractionType;
        }

        public IImplementationConfiguration[] GetImplementations()
        {
            throw new ForbiddenAbstractionException(abstractionType);
        }

        private readonly Type abstractionType;
    }
}