using System;

using GroboContainer.Core;

namespace GroboContainer.Tests
{
    public class TestClassWrapperCreator: IClassWrapperCreator
    {
        public Type Wrap(Type implementationType)
        {
            return null;
        }

        public object WrapAndCreate(object instance)
        {
            return instance;
        }

        public object UnWrap(object instance)
        {
            return instance;
        }
    }
}