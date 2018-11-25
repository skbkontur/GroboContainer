using GroboContainer.Core;
using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.Abstractions.AutoConfiguration;
using GroboContainer.Impl.ChildContainersSupport.Selectors;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.Impl.Implementations;
using GroboContainer.New;

namespace GroboContainer.Impl.ChildContainersSupport
{
    public class CompositeContainerContext : IContainerContext
    {
        public CompositeContainerContext(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator,
                                         IContainerSelector containerSelector)
        {
            Configuration = configuration;
            ClassWrapperCreator = classWrapperCreator;
            typesHelper = new TypesHelper();

            var funcHelper = new FuncHelper();
            FuncBuilder = new FuncBuilder();
            var classCreator = new ClassCreator(funcHelper);
            var constructorSelector = new ConstructorSelector();
            CreationContext = new CreationContext(classCreator, constructorSelector, classWrapperCreator);

            var implementationTypesCollection = new ImplementationTypesCollection(configuration.GetTypesToScan(), typesHelper);
            ImplementationCache = new ImplementationCache();
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, ImplementationCache);
            ImplementationConfigurationCache = new ImplementationConfigurationCache();
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                                  ImplementationConfigurationCache);
            compositeCollection = new CompositeCollection(new[] {new AbstractionConfigurationCollection(factory)},
                                                          containerSelector);
            compositeCollection.Add(typeof(IContainer),
                                    new StupidAbstractionConfiguration(
                                        new ContainerImplementationConfiguration()));
        }

        private CompositeContainerContext(CompositeContainerContext source)
        {
            Configuration = source.Configuration;
            typesHelper = source.typesHelper;
            FuncBuilder = source.FuncBuilder;
            CreationContext = source.CreationContext;

            //NOTE чтобы дочерние контейнеры не тормозили, используем ту же AbstractionsCollection
            abstractionsCollection = source.abstractionsCollection;
            ImplementationCache = source.ImplementationCache;

            ImplementationConfigurationCache = new ImplementationConfigurationCache();
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                                  ImplementationConfigurationCache);
            //NOTE для каждого экземпляра контейнера должна быть своя AbstractionConfigurationCollection
            var abstractionConfigurationCollection = new AbstractionConfigurationCollection(factory);
            compositeCollection =
                source.compositeCollection.MakeChildCollection(abstractionConfigurationCollection);
            compositeCollection.Add(typeof(IContainer),
                                    new StupidAbstractionConfiguration(
                                        new ContainerImplementationConfiguration()));
        }

        private readonly IAbstractionsCollection abstractionsCollection;
        private readonly CompositeCollection compositeCollection;
        private readonly ITypesHelper typesHelper;

        #region IContainerContext Members

        public IImplementationCache ImplementationCache { get; }
        public IImplementationConfigurationCache ImplementationConfigurationCache { get; }
        public IClassWrapperCreator ClassWrapperCreator { get; }

        public IFuncBuilder FuncBuilder { get; }

        public ICreationContext CreationContext { get; }

        public IAbstractionConfigurationCollection AbstractionConfigurationCollection => compositeCollection;

        public IContainerContext MakeChildContext()
        {
            return new CompositeContainerContext(this);
        }

        public IContainerConfiguration Configuration { get; }

        #endregion
    }
}