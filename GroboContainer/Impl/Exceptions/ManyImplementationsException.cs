using System;
using System.Text;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Exceptions
{
    public class ManyImplementationsException : Exception
    {
        public ManyImplementationsException(Type requested, IImplementationConfiguration[] existingConfigs)
            : base(
                $"Тип {requested} имеет более 1 реализации. Реализации:\r\n{DumpExistingTypes(existingConfigs)}"
                )
        {
        }

        private static string DumpExistingTypes(IImplementationConfiguration[] existingConfigs)
        {
            var stringBuilder = new StringBuilder();
            foreach (var configuration in existingConfigs)
            {
                stringBuilder.AppendFormat("{0}\r\n", configuration.ObjectType.FullName);
            }
            return stringBuilder.ToString();
        }
    }
}