using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GroboContainer.Infection
{
    [SuppressMessage("ReSharper", "RedundantAttributeUsageProperty")]
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class ContainerConstructorAttribute : Attribute
    {
        public ContainerConstructorAttribute(params Type[] parameterTypes)
        {
            this.parameterTypes = parameterTypes;
        }

        public IEnumerable<Type> GetParameterTypes()
        {
            return parameterTypes;
        }

        private readonly Type[] parameterTypes;
    }
}