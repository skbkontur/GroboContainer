using System;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.Abstractions.AutoConfiguration
{
    public class AutoAbstractionConfigurationFactory : IAutoAbstractionConfigurationFactory
    {
        private readonly IAbstractionsCollection abstractionsCollection;
        private readonly IImplementationConfigurationCache implConfigCache;
        private readonly ITypesHelper typesHelper;

        public AutoAbstractionConfigurationFactory(ITypesHelper typesHelper,
                                                   IAbstractionsCollection abstractionsCollection,
                                                   IImplementationConfigurationCache implConfigCache)
        {
            this.typesHelper = typesHelper;
            this.abstractionsCollection = abstractionsCollection;
            this.implConfigCache = implConfigCache;
        }

        #region IAutoAbstractionConfigurationFactory Members

        public IAbstractionConfiguration CreateByType(Type abstractionType)
        {
            if (typesHelper.IsIgnoredAbstraction(abstractionType))
                return new StupidAbstractionConfiguration(new ForbiddenImplementationConfiguration(abstractionType));
            return new AutoAbstractionConfiguration(abstractionType, abstractionsCollection, implConfigCache);
        }

        #endregion
    }
}