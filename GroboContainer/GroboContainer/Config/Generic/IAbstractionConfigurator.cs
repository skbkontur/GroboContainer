namespace GroboContainer.Config.Generic
{
    public interface IAbstractionConfigurator<in T>
    {
        void UseInstances(params T[] instances);
        void Fail();
    }
}