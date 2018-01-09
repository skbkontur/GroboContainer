using System;
using System.Reflection;

namespace GroboContainer.Impl.Exceptions
{
    public class BadContainerConstructorAttributeException : Exception
    {
        public BadContainerConstructorAttributeException(ConstructorInfo constructor, Type type)
            : base(string.Format(
                       "¬ аттрибуте ContainerConstructor конструктора {0} {1} тип параметра {2} должен присутствовать не более 1 раза",
                       constructor.ReflectedType, constructor, type))
        {
        }

        public BadContainerConstructorAttributeException(ConstructorInfo constructor)
            : base(string.Format(
                       "“ипы, описанные в аттрибуте ContainerConstructor конструктора {0} {1}, не соответствуют типам параметров конструктора",
                       constructor.ReflectedType, constructor))
        {
        }
    }
}