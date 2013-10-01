using System;

namespace GroboContainer.Core
{
    public interface IClassWrapperCreator
    {
        Type Wrap(Type implementationType);
        object WrapAndCreate(object instance);
        object UnWrap(object instance);
    }
}