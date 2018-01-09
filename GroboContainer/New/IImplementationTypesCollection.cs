using System;

namespace GroboContainer.New
{
    public interface IImplementationTypesCollection
    {
        Type[] GetImplementationTypes(Type abstractionType);
    }
}