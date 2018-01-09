using System;

namespace GroboContainer.New
{
    public interface IImplementationCache
    {
        IImplementation GetOrCreate(Type implementationType);
    }
}