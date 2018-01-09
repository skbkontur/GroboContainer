using System;

using GroboContainer.Core;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public class CreationContext : ICreationContext
    {
        public CreationContext(IClassCreator classCreator, IConstructorSelector constructorSelector, IClassWrapperCreator classWrapperCreator)
        {
            this.classCreator = classCreator;
            this.constructorSelector = constructorSelector;
            this.classWrapperCreator = classWrapperCreator;
        }

        #region ICreationContext Members

        public IClassFactory BuildFactory(Type implementationType, Type[] parameterTypes)
        {
            return classCreator.BuildFactory(constructorSelector.GetConstructor(implementationType, parameterTypes), classWrapperCreator == null ? null : classWrapperCreator.Wrap(implementationType));
        }

        #endregion

        private readonly IClassCreator classCreator;
        private readonly IConstructorSelector constructorSelector;
        private readonly IClassWrapperCreator classWrapperCreator;
    }
}