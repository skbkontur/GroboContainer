namespace GroboContainer.Config
{
    public interface IAbstractionConfigurator
    {
        void UseInstances(params object[] instances);
        void Fail();
    }
}