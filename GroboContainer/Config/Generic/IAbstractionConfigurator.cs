namespace GroboContainer.Config.Generic
{
    public interface IAbstractionConfigurator<in T>
    {
        void UseInstances(params T[] instances);
        void Fail();
        void UseType<TImpl>() where TImpl : T;
    }
}