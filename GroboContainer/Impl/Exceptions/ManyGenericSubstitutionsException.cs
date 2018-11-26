using System;
using System.Text;

namespace GroboContainer.Impl.Exceptions
{
    public class ManyGenericSubstitutionsException : Exception
    {
        public ManyGenericSubstitutionsException(Type genericType, Type genericParameterType, Type[] substitutionTypes)
            : base($"Generic parameter type {genericParameterType} for generic type {genericType} has more than one type for substitution:{Environment.NewLine}{Dump(substitutionTypes)}")
        {
        }

        private static string Dump(Type[] substitutionTypes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var substitutionType in substitutionTypes)
                stringBuilder.AppendLine(substitutionType.FullName);
            return stringBuilder.ToString();
        }
    }
}