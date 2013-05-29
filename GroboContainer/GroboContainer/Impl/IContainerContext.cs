using GroboContainer.Impl.Abstractions;
using GroboContainer.Impl.ClassCreation;
using GroboContainer.New;

namespace GroboContainer.Impl
{
    public interface IContainerContext
    {
        IFuncBuilder FuncBuilder { get; }
        ICreationContext CreationContext { get; }
        IAbstractionConfigurationCollection AbstractionConfigurationCollection { get; }
        IContainerContext MakeChildContext();
        //IContainerConfigurator ContainerConfigurator { get; }
        //IContainerConfiguration Configuration { get; }
    }
}