using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Daikin.DotNetLib.Network
{
    public static class Xml
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null) return string.Empty;

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlSerializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred attempting to Serialize", ex);
            }
        }

        public static T ObjectFromString<T>(string xmlString, bool ignoreComments = true, bool ignoreWhiteSpace = true)
        {
            if (xmlString == null) return default(T);
            var xmlReaderSettings = new XmlReaderSettings { IgnoreComments = ignoreComments, IgnoreWhitespace = ignoreWhiteSpace };
            var stringReader = new StringReader(xmlString);
            var xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xmlReader); // W3C defines boolean data type as 'true', 'false', '0', and '1' - 'False' and 'True' are not booleans

            //var serializer = new XmlSerializer(typeof(T));
            //return (T)serializer.Deserialize(new StringReader(xml));
        }

        public static string ObjectToString<T>(T item, NamespaceHandling namespaceHandling = NamespaceHandling.OmitDuplicates, bool omitXmlDeclaration = true)
        {
            var sb = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings { NamespaceHandling = namespaceHandling, OmitXmlDeclaration = omitXmlDeclaration };
            var xmlWriter = XmlWriter.Create(sb, xmlWriterSettings);
            var xmlNamespace = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, "") });
            new XmlSerializer(typeof(T)).Serialize(xmlWriter, item, xmlNamespace);
            return sb.ToString();
        }

        public static T ObjectFromFile<T>(string fileAndPath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(fileAndPath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        public static void ObjectToFile<T>(T obj, string fileAndPath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(fileAndPath, FileMode.Create))
            {
                serializer.Serialize(stream, obj);
            }
        }

        public static string Pretty(string xml)
        {
            string resultXml;

            var memoryStream = new MemoryStream();
            var xmlWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
            var xmlDocument = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                xmlDocument.LoadXml(xml);

                xmlWriter.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                xmlDocument.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
                memoryStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                memoryStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                var sReader = new StreamReader(memoryStream);

                // Extract the text from the StreamReader.
                var formattedXml = sReader.ReadToEnd();

                resultXml = formattedXml;
            }
            catch (XmlException)
            {
                resultXml = xml;
            }

            xmlWriter.Close();
            memoryStream.Close();

            return resultXml;
        }
    }
}
