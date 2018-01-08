using System;
using System.Text;
using System.Xml;

namespace Tests.NMockHelpers
{
    public static class XmlHelpers
    {
        private static T TryGetChildNode<T>(this XmlNode parent, string localName) where T : XmlNode
        {
            return parent.TryGetChildNode<T>(localName, null);
        }

        private static T TryGetChildNode<T>(this XmlNode parent, string localName, string namespaceUri) where T : XmlNode
        {
            foreach (XmlNode childNode in parent.ChildNodes)
            {
                if (localName.Equals(childNode.LocalName, StringComparison.OrdinalIgnoreCase) && childNode is T && (namespaceUri == null || childNode.NamespaceURI == namespaceUri))
                    return (T) childNode;
            }
            return default (T);
        }

        private static string FormattedOuterXml(this XmlNode node)
        {
            var output = new StringBuilder();
            var w = XmlWriter.Create(output, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = !node.HasXmlDeclaration()
            });
            node.WriteTo(w);
            w.Flush();
            return output.ToString();
        }

        private static bool HasXmlDeclaration(this XmlNode node)
        {
            return node.TryGetChildNode<XmlDeclaration>("xml") != null;
        }

        public static string ReformatXml(this string xml)
        {
            return CreateXml(xml).FormattedOuterXml();
        }

        private static XmlDocument CreateXml(Action<XmlDocument> loadAction)
        {
            var xmlDocument = new XmlDocument();
            loadAction(xmlDocument);
            return xmlDocument;
        }

        private static XmlDocument CreateXml(string xml)
        {
            return CreateXml(x => x.LoadXml(xml));
        }
    }
}