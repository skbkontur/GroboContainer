using System;
using System.Collections.Generic;

namespace GroboContainer.Infection
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class ContainerConstructorAttribute : Attribute
    {
        private readonly Type[] parameterTypes;

        public ContainerConstructorAttribute(params Type[] parameterTypes)
        {
            this.parameterTypes = parameterTypes;
        }

        public IEnumerable<Type> GetParameterTypes()
        {
            return parameterTypes;
        }
    }
}