using System;
using System.Text;

using GroboContainer.Impl.Implementations;

namespace GroboContainer.Impl.Exceptions
{
    public class ManyImplementationsException : Exception
    {
        public ManyImplementationsException(Type requested, IImplementationConfiguration[] existingConfigs)
            : base($"There are several implementations for type {requested}:{Environment.NewLine}{DumpExistingTypes(existingConfigs)}")
        {
        }

        private static string DumpExistingTypes(IImplementationConfiguration[] existingConfigs)
        {
            var stringBuilder = new StringBuilder();
            foreach (var configuration in existingConfigs)
                stringBuilder.AppendLine(configuration.ObjectType.FullName);
            return stringBuilder.ToString();
        }
    }
}