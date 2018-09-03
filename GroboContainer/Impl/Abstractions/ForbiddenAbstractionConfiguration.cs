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

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            throw new ForbiddenAbstractionException(abstractionType);
        }

        #endregion

        private readonly Type abstractionType;
    }
}