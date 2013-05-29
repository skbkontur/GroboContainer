using System;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public class CreationContext : ICreationContext
    {
        private readonly IClassCreator classCreator;
        private readonly IConstructorSelector constructorSelector;

        public CreationContext(IClassCreator classCreator, IConstructorSelector constructorSelector)
        {
            this.classCreator = classCreator;
            this.constructorSelector = constructorSelector;
        }

        #region ICreationContext Members

        public IClassFactory BuildFactory(Type implementationType, Type[] parameterTypes)
        {
            return classCreator.BuildFactory(constructorSelector.GetConstructor(implementationType, parameterTypes));
        }

        #endregion
    }
}