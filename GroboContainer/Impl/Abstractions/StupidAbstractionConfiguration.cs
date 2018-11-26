using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class StupidAbstractionConfiguration : IAbstractionConfiguration
    {
        public StupidAbstractionConfiguration(params IImplementationConfiguration[] implementationConfigurations)
        {
            this.implementationConfigurations = implementationConfigurations;
        }

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        private readonly IImplementationConfiguration[] implementationConfigurations;
    }
}