using System;
using System.Text;

namespace GroboContainer.OldHlam
{
    public static class ContractHelpers
    {
        public static string ConvertToString(this string[] contractsArray)
        {
            if (ReferenceEquals(null, contractsArray))
                return null;
            var copyArray = (string[]) contractsArray.Clone();
            Array.Sort(copyArray);

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < contractsArray.Length; i++)
                stringBuilder.Append(copyArray[i]).Append((char) 0);
            return stringBuilder.ToString();
        }
    }
}