using System;
using System.Text;

namespace GroboContainer.Impl.Exceptions
{
    public class ManyGenericSubstitutionsException : Exception
    {
        public ManyGenericSubstitutionsException(Type genericType, Type genericParameterType, Type[] substitutionTypes)
            : base(
                string.Format("Параметр {0} типа {1} имеет более 1 типа для подстановки. Типы для подстановки:\r\n{2}",
                              genericParameterType,
                              genericType,
                              Dump(substitutionTypes)
                    )
                )
        {
        }

        private static string Dump(Type[] substitutionTypes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var substitutionType in substitutionTypes)
            {
                stringBuilder.AppendFormat("{0}\r\n", substitutionType.FullName);
            }
            return stringBuilder.ToString();
        }
    }
}