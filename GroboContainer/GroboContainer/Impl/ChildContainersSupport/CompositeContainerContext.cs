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
        private readonly IAbstractionsCollection abstractionsCollection;
        private readonly CompositeCollection compositeCollection;
        private readonly IContainerConfiguration configuration;
        private readonly ITypesHelper typesHelper;

        public CompositeContainerContext(IContainerConfiguration configuration, IClassWrapperCreator classWrapperCreator,
                                         IContainerSelector containerSelector)
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
            abstractionsCollection = new AbstractionsCollection(implementationTypesCollection, implementationCache);
            var implementationConfigurationCache = new ImplementationConfigurationCache();
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                                  implementationConfigurationCache);
            compositeCollection = new CompositeCollection(new[] {new AbstractionConfigurationCollection(factory)},
                                                          containerSelector);
            compositeCollection.Add(typeof (IContainer),
                                    new StupidAbstractionConfiguration(
                                        new ContainerImplementationConfiguration()));
        }

        private CompositeContainerContext(CompositeContainerContext source)
        {
            configuration = source.configuration;
            typesHelper = source.typesHelper;
            FuncBuilder = source.FuncBuilder;
            CreationContext = source.CreationContext;

            //NOTE чтобы дочерние контейнеры не тормозили, используем ту же AbstractionsCollection
            abstractionsCollection = source.abstractionsCollection;

            var implementationConfigurationCache = new ImplementationConfigurationCache();
            var factory = new AutoAbstractionConfigurationFactory(typesHelper, abstractionsCollection,
                                                                  implementationConfigurationCache);
            //NOTE для каждого экземпляра контейнера должна быть своя AbstractionConfigurationCollection
            var abstractionConfigurationCollection = new AbstractionConfigurationCollection(factory);
            compositeCollection =
                source.compositeCollection.MakeChildCollection(abstractionConfigurationCollection);
            compositeCollection.Add(typeof (IContainer),
                                    new StupidAbstractionConfiguration(
                                        new ContainerImplementationConfiguration()));
        }

        #region IContainerContext Members

        public IClassWrapperCreator ClassWrapperCreator { get; private set; }

        public IFuncBuilder FuncBuilder { get; private set; }

        public ICreationContext CreationContext { get; private set; }

        public IAbstractionConfigurationCollection AbstractionConfigurationCollection
        {
            get { return compositeCollection; }
        }

        public IContainerContext MakeChildContext()
        {
            return new CompositeContainerContext(this);
        }

        public IContainerConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion
    }
}