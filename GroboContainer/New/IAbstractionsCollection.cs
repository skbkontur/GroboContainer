using System;

namespace GroboContainer.New
{
    public interface IAbstractionsCollection
    {
        IAbstraction Get(Type abstractionType);
    }
}