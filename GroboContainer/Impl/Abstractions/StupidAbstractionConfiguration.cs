using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class StupidAbstractionConfiguration : IAbstractionConfiguration
    {
        private readonly IImplementationConfiguration[] implementationConfigurations;

        public StupidAbstractionConfiguration(params IImplementationConfiguration[] implementationConfigurations)
        {
            this.implementationConfigurations = implementationConfigurations;
        }

        #region IAbstractionConfiguration Members

        public IImplementationConfiguration[] GetImplementations()
        {
            return implementationConfigurations;
        }

        #endregion
    }
}