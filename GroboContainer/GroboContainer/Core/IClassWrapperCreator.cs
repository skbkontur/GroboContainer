using System;

namespace GroboContainer.Core
{
    public interface IClassWrapperCreator
    {
        Type Wrap(Type implementationType);
        object UnWrap(object instance);
    }
}