using DnDGen.Core.Mappers.Percentiles;
using DnDGen.Core.Tables;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace DnDGen.Core.Tests.Unit.Mappers.Percentiles
{
    [TestFixture]
    public class PercentileXmlMapperTests
    {
        private const string tableName = "PercentileXmlMapperTests";

        private PercentileMapper mapper;
        private Mock<StreamLoader> mockStreamLoader;
        private string filename;
        private string contents;

        [SetUp]
        public void Setup()
        {
            filename = tableName + ".xml";
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            mockStreamLoader = new Mock<StreamLoader>();
            mockStreamLoader.Setup(l => l.LoadFor(filename)).Returns(() => GetStream());

            mapper = new PercentileXmlMapper(mockStreamLoader.Object);
        }

        private Stream GetStream()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        [Test]
        public void AppendXmlFileExtensionToTableName()
        {
            mapper.Map(tableName);
            mockStreamLoader.Verify(l => l.LoadFor(filename), Times.Once);
        }

        [Test]
        public void LoadXmlFromStream()
        {
            var table = mapper.Map(tableName);

            Assert.That(table[1], Is.EqualTo("one through five"));
            Assert.That(table[2], Is.EqualTo("one through five"));
            Assert.That(table[3], Is.EqualTo("one through five"));
            Assert.That(table[4], Is.EqualTo("one through five"));
            Assert.That(table[5], Is.EqualTo("one through five"));
            Assert.That(table[6], Is.EqualTo("six only"));

            for (var i = 7; i < 100; i++)
                Assert.That(table[i], Is.Empty);

            Assert.That(table.Count(), Is.EqualTo(100));
        }

        [Test]
        public void EmptyPercentilesThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} is empty"));
        }

        [Test]
        public void WrongRootNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <wrongnode>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </wrongnode>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Table {tableName} is not a percentile table (does not have \"percentiles\" as root node)"));
        }

        [Test]
        public void WrongPercentileNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <wrongnode>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </wrongnode>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} contains an invalid percentile node called \"wrongnode\""));
        }

        [Test]
        public void WrongInnerNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <wrongnode>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <wrongnode>wrong node</wrongnode>
                                 <upper>7</upper>
                             </wrongnode>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} contains an invalid percentile node called \"wrongnode\""));
        }

        [Test]
        public void DuplicateRollsThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>7</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has duplicate results for 7"));
        }

        [Test]
        public void MissingRollThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>4</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>99</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} is missing results for 5, 100"));
        }

        [Test]
        public void ExtraRollThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>0</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>101</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has extra results for 0, 101"));
        }

        [Test]
        public void MissingLowerThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }

        [Test]
        public void MissingContentThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }

        [Test]
        public void MissingUpperThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }

        [Test]
        public void DuplicateLowerThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <lower>5</lower>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }

        [Test]
        public void DuplicateContentThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <content>wrong content</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <lower>7</lower>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }

        [Test]
        public void DuplicateUpperThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <percentiles>
                             <percentile>
                                 <lower>1</lower>
                                 <content>one through five</content>
                                 <upper>5</upper>
                             </percentile>
                             <percentile>
                                 <lower>6</lower>
                                 <content>six only</content>
                                 <upper>6</upper>
                             </percentile>
                             <percentile>
                                 <upper>7</upper>
                                 <content></content>
                                 <upper>100</upper>
                             </percentile>
                         </percentiles>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Percentile table {tableName} has a malformed percentile node"));
        }
    }
}