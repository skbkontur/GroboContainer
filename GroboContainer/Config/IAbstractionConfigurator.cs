using System;

namespace GroboContainer.Config
{
    public interface IAbstractionConfigurator
    {
        void UseInstances(params object[] instances);
        void Fail();
        void UseType(Type type);
        void UseTypes(Type[] types);
    }
}