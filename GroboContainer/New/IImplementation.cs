using System;
using GroboContainer.Impl.ClassCreation;

namespace GroboContainer.New
{
    public interface IImplementation
    {
        Type ObjectType { get; }
        IClassFactory GetFactory(Type[] parameterTypes, ICreationContext creationContext);
    }
}