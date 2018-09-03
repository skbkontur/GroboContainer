using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Abstractions
{
    public class StupidAbstractionConfiguration : IAbstractionConfiguration
    {
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

        private readonly IImplementationConfiguration[] implementationConfigurations;
    }
}