using System;
using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class ForbiddenAbstractionConfiguration : IAbstractionConfiguration
    {
        private readonly Type abstractionType;

        public ForbiddenAbstractionConfiguration(Type abstractionType)
        {
            this.abstractionType = abstractionType;
        }

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            throw new ForbiddenAbstractionException(abstractionType);
        }

        #endregion
    }
}