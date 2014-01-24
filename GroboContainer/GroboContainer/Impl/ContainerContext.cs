using System;
using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl
{
    public class ContainerContext : IContainerContext
    {
        private readonly IAbstractionsCollection abstractionsCollection;
        private readonly IContainerConfiguration configuration;
        private readonly ITypesHelper typesHelper;

        public ContainerContext(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator)
        {
            this.configuration = configuration;
            ClassWrapperCreator = classWrapperCreator;
            typesHelper = new TypesHelper();

            var funcHelper = new FuncHelper();
            FuncBuilder = new FuncBuilder();
            var classCreator = new ClassCreator(funcHelper);
            var constructorSelector = new ConstructorSelector();
            CreationContext = new CreationContext(classCreator, constructorSelector, classWrapperCreator);

            var implementationTypesCollection = new ImplementationTypesCollection(configuration, typesHelper);
            var implementationCache = new ImplementationCache();
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, implementationCache); //g
            var implementationConfigurationCache = new ImplementationConfigurationCache(); //l
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                                  implementationConfigurationCache);
            AbstractionConfigurationCollection = new AbstractionConfigurationCollection(factory);
            AbstractionConfigurationCollection.Add(typeof (IContainer),
                                                   new StupidAbstractionConfiguration(
                                                       new ContainerImplementationConfiguration()));
        }

        public IClassWrapperCreator ClassWrapperCreator { get; private set; }

        #region IContainerContext Members

        public IFuncBuilder FuncBuilder { get; private set; } //s, no state

        public ICreationContext CreationContext { get; private set; } //s, no statexx
        public IAbstractionConfigurationCollection AbstractionConfigurationCollection { get; private set; }

        public IContainerContext MakeChildContext()
        {
            throw new NotSupportedException("Childs");
        }

        public IContainerConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion
    }
}