using System;
using System.Reflection;

namespace GroboContainer.Impl.Exceptions
{
    public class BadContainerConstructorAttributeException : Exception
    {
        public BadContainerConstructorAttributeException(ConstructorInfo constructor, Type type)
            : base($"В аттрибуте ContainerConstructor конструктора {constructor.ReflectedType} {constructor} тип параметра {type} должен присутствовать не более 1 раза")
        {
        }

        public BadContainerConstructorAttributeException(ConstructorInfo constructor)
            : base($"Типы, описанные в аттрибуте ContainerConstructor конструктора {constructor.ReflectedType} {constructor}, не соответствуют типам параметров конструктора")
        {
        }
    }
}