using DnDGen.Infrastructure.Tables;
using System;
using System.Collections.Generic;
using System.Xml;

namespace DnDGen.Infrastructure.Mappers.Collections
{
    internal class CollectionXmlMapper : CollectionMapper
    {
        private readonly StreamLoader streamLoader;

        public CollectionXmlMapper(StreamLoader streamLoader)
        {
            this.streamLoader = streamLoader;
        }

        public Dictionary<string, IEnumerable<string>> Map(string assemblyName, string tableName)
        {
            var filename = tableName + ".xml";
            var mappedTable = new Dictionary<string, IEnumerable<string>>();
            var xmlDocument = new XmlDocument();

            using (var stream = streamLoader.LoadFor(assemblyName, filename))
                xmlDocument.Load(stream);

            if (xmlDocument.DocumentElement.LocalName != "collections")
                throw new Exception($"Table {tableName} is not a collection table (does not have \"collections\" as root node)");

            var collectionNodes = xmlDocument.DocumentElement.ChildNodes;

            foreach (XmlNode node in collectionNodes)
            {
                if (node.LocalName != "collection")
                    throw new Exception($"Collection table {tableName} contains an invalid collection node called \"{node.LocalName}\"");

                var nameNodes = node.SelectNodes("name");
                var entryNodes = node.SelectNodes("entry");

                if (nameNodes.Count != 1 || nameNodes.Count + entryNodes.Count != node.ChildNodes.Count)
                    throw new Exception($"Collection table {tableName} has a malformed collection node");

                var name = node.SelectSingleNode("name").InnerText;
                if (mappedTable.ContainsKey(name))
                    throw new Exception($"Collection table {tableName} has duplicate collections of {name}");

                var entries = new List<string>();

                foreach (XmlNode entryNode in entryNodes)
                    entries.Add(entryNode.InnerText);

                mappedTable.Add(name, entries);
            }

            return mappedTable;
        }
    }
}