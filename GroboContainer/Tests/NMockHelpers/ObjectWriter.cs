using System;
using System.Xml;

namespace Tests.NMockHelpers
{
    public class ObjectWriter
    {
        private readonly SimpleTypeWriter simpleTypeWriter;
        private readonly XmlWriter writer;

        public ObjectWriter(XmlWriter writer)
        {
            this.writer = writer;
            simpleTypeWriter = new SimpleTypeWriter();
        }

        public void Write(Type type, object value)
        {
            Write(type, value, "root");
        }

        private void Write(Type type, object value, string name)
        {
            writer.WriteStartElement(name);
            DoWrite(type, value);
            writer.WriteEndElement();
        }

        private void DoWrite(Type type, object value)
        {
            if (TryWriteNullValue(value) || TryWriteNullableTypeValue(type, value) || TryWriteSimpleTypeValue(type, value) || TryWriteArrayTypeValue(type, value))
                return;
            WriteComplexTypeValue(type, value);
        }

        private void WriteComplexTypeValue(Type type, object value)
        {
            foreach (var field in TypeHelpers.GetInstanceFields(type))
            {
                if (!field.FieldType.IsInterface)
                    Write(field.FieldType, field.GetValue(value), FieldNameToTagName(field.Name));
            }
        }

        private static string FieldNameToTagName(string name)
        {
            if (name.EndsWith(">k__BackingField"))
                return name.Substring(1, name.IndexOf('>') - 1);
            return name;
        }

        private bool TryWriteArrayTypeValue(Type type, object value)
        {
            if (!type.IsArray)
                return false;
            writer.WriteAttributeString(nameof (type), "array");
            var array = (Array) value;
            if (array.Rank > 1)
                throw new NotSupportedException("array with rank > 1");
            var elementType = type.GetElementType();
            for (var index = 0; index < array.Length; ++index)
            {
                var obj = array.GetValue(index);
                Write(elementType, obj, "item");
            }
            return true;
        }

        private bool TryWriteSimpleTypeValue(Type type, object value)
        {
            var str = simpleTypeWriter.TryWrite(type, value);
            if (str == null)
                return false;
            writer.WriteValue(str);
            return true;
        }

        private bool TryWriteNullableTypeValue(Type type, object value)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof (Nullable<>))
                return false;
            if (!(bool) type.GetProperty("HasValue").GetGetMethod().Invoke(value, new object[0]))
            {
                WriteNull();
            }
            else
            {
                var obj = type.GetProperty("Value").GetGetMethod().Invoke(value, new object[0]);
                DoWrite(type.GetGenericArguments()[0], obj);
            }
            return true;
        }

        private bool TryWriteNullValue(object value)
        {
            if (value != null)
                return false;
            WriteNull();
            return true;
        }

        private void WriteNull()
        {
            writer.WriteAttributeString("type", "null");
        }
    }
}