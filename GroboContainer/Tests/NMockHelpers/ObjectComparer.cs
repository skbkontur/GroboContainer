using System;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Tests.NMockHelpers
{
    public static class ObjectComparer
    {
        public static void AssertEqualsTo<T>(this T actual, T expected)
        {
            var str1 = "<root></root>".ReformatXml();
            var str2 = ObjectToString(typeof(T), expected);
            Assert.AreNotEqual(str2.ReformatXml(), str1, "bug(expected)");
            var str3 = ObjectToString(typeof(T), actual);
            Assert.AreNotEqual(str3.ReformatXml(), str1, "bug(actual)");
            Assert.AreEqual(str2, str3, "actual:\n{0}\nexpected:\n{1}", str3, str2);
        }

        private static string ObjectToString(Type type, object instance)
        {
            if (type.IsInterface)
                throw new InvalidOperationException(string.Format("Cannot compare interface type={0}", type.Name));
            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            });
            new ObjectWriter(writer).Write(type, instance);
            writer.Flush();
            return output.ToString();
        }
    }
}