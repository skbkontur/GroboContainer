using System;

using GroboContainer.Config.Generic;

namespace GroboContainer.Config
{
    public interface IContainerConfigurator
    {
        IAbstractionConfigurator<T> ForAbstraction<T>();
        IAbstractionConfigurator ForAbstraction(Type abstractionType);
    }
}