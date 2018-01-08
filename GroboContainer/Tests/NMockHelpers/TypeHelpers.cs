using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tests.NMockHelpers
{
    public static class TypeHelpers
    {
        private static List<FieldInfo> GetTypeInstanceFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
            var fieldInfoList = new List<FieldInfo>();
            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.IsStatic && !fieldInfo.IsLiteral)
                    fieldInfoList.Add(fieldInfo);
            }
            return fieldInfoList;
        }

        public static List<FieldInfo> GetInstanceFields(Type type)
        {
            var fieldInfoList = new List<FieldInfo>();
            for (; type != null as Type; type = type.BaseType)
                fieldInfoList.AddRange(GetTypeInstanceFields(type));
            return fieldInfoList;
        }
    }
}