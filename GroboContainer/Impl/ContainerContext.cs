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
        public ContainerContext(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator)
        {
            Configuration = configuration;
            ClassWrapperCreator = classWrapperCreator;
            ITypesHelper typesHelper = new TypesHelper();

            var funcHelper = new FuncHelper();
            FuncBuilder = new FuncBuilder();
            var classCreator = new ClassCreator(funcHelper);
            var constructorSelector = new ConstructorSelector();
            CreationContext = new CreationContext(classCreator, constructorSelector, classWrapperCreator);

            var implementationTypesCollection = new ImplementationTypesCollection(configuration.GetTypesToScan(), typesHelper);
            ImplementationCache = new ImplementationCache();
            IAbstractionsCollection abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, ImplementationCache);
            ImplementationConfigurationCache = new ImplementationConfigurationCache(); //l
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection, ImplementationConfigurationCache);
            AbstractionConfigurationCollection = new AbstractionConfigurationCollection(factory);
            AbstractionConfigurationCollection.Add(typeof(IContainer), new StupidAbstractionConfiguration(new ContainerImplementationConfiguration()));
        }

        public IClassWrapperCreator ClassWrapperCreator { get; }
        public IImplementationConfigurationCache ImplementationConfigurationCache { get; }
        public IImplementationCache ImplementationCache { get; }

        public IFuncBuilder FuncBuilder { get; } //s, no state

        public ICreationContext CreationContext { get; } //s, no statexx
        public IAbstractionConfigurationCollection AbstractionConfigurationCollection { get; }

        public IContainerContext MakeChildContext()
        {
            throw new NotSupportedException("Childs");
        }

        public IContainerConfiguration Configuration { get; }
    }
}