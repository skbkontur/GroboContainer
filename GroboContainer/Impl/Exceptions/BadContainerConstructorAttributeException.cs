using System;
using System.Reflection;

namespace GroboContainer.Impl.Exceptions
{
    public class BadContainerConstructorAttributeException : Exception
    {
        public BadContainerConstructorAttributeException(ConstructorInfo constructor, Type type)
            : base($"Parameter type {type} must not appear in list of types in [ContainerConstructor] attribute for constructor {constructor.ReflectedType} {constructor} more than once")
        {
        }

        public BadContainerConstructorAttributeException(ConstructorInfo constructor)
            : base($"Types listed in [ContainerConstructor] attribute for constructor {constructor.ReflectedType} {constructor} do not match with its argument types")
        {
        }
    }
}