using DnDGen.Infrastructure.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace DnDGen.Infrastructure.Mappers.Percentiles
{
    internal class PercentileXmlMapper : PercentileMapper
    {
        private readonly StreamLoader streamLoader;

        public PercentileXmlMapper(StreamLoader streamLoader)
        {
            this.streamLoader = streamLoader;
        }

        public Dictionary<int, string> Map(string tableName)
        {
            var filename = tableName + ".xml";
            var mappedTable = new Dictionary<int, string>();
            var xmlDocument = new XmlDocument();

            using (var stream = streamLoader.LoadFor(filename))
                xmlDocument.Load(stream);

            if (xmlDocument.DocumentElement.LocalName != "percentiles")
                throw new Exception($"Table {tableName} is not a percentile table (does not have \"percentiles\" as root node)");

            var percentileNodes = xmlDocument.DocumentElement.ChildNodes;

            if (percentileNodes.Count == 0)
                throw new Exception($"Percentile table {tableName} is empty");

            foreach (XmlNode node in percentileNodes)
            {
                if (node.LocalName != "percentile")
                    throw new Exception($"Percentile table {tableName} contains an invalid percentile node called \"{node.LocalName}\"");

                var lowerNodes = node.SelectNodes("lower");
                var contentNodes = node.SelectNodes("content");
                var upperNodes = node.SelectNodes("upper");

                if (lowerNodes.Count != 1 || contentNodes.Count != 1 || upperNodes.Count != 1 || node.ChildNodes.Count != 3)
                    throw new Exception($"Percentile table {tableName} has a malformed percentile node");

                var lowerLimit = Convert.ToInt32(lowerNodes[0].InnerText);
                var content = contentNodes[0].InnerText;
                var upperLimit = Convert.ToInt32(upperNodes[0].InnerText);

                for (var roll = lowerLimit; roll <= upperLimit; roll++)
                {
                    if (mappedTable.ContainsKey(roll))
                        throw new Exception($"Percentile table {tableName} has duplicate results for {roll}");

                    mappedTable.Add(roll, content);
                }
            }

            var rolls = Enumerable.Range(1, 100);
            var missing = rolls.Except(mappedTable.Keys);

            if (missing.Any())
                throw new Exception($"Percentile table {tableName} is missing results for {string.Join(", ", missing)}");

            var extras = mappedTable.Keys.Except(rolls);

            if (extras.Any())
                throw new Exception($"Percentile table {tableName} has extra results for {string.Join(", ", extras)}");

            return mappedTable;
        }
    }
}