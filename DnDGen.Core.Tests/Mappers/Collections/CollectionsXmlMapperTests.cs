using DnDGen.Core.Mappers.Collections;
using DnDGen.Core.Tables;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace DnDGen.Core.Tests.Mappers.Collections
{
    [TestFixture]
    public class CollectionsXmlMapperTests
    {
        private const string tableName = "CollectionsXmlMapperTests";

        private string fileName;
        private CollectionsMapper mapper;
        private Mock<StreamLoader> mockStreamLoader;
        private string contents;

        [SetUp]
        public void Setup()
        {
            fileName = tableName + ".xml";
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <collection>
                                 <name>second name</name>
                                 <entry>third item</entry>
                             </collection>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </collections>";

            mockStreamLoader = new Mock<StreamLoader>();
            mockStreamLoader.Setup(l => l.LoadFor(fileName)).Returns(() => GetStream());

            mapper = new CollectionsXmlMapper(mockStreamLoader.Object);
        }

        [Test]
        public void AppendXmlFileExtensionToTableName()
        {
            mapper.Map(tableName);
            mockStreamLoader.Verify(l => l.LoadFor(fileName), Times.Once);
        }

        [Test]
        public void LoadXmlFromStream()
        {
            var table = mapper.Map(tableName);

            var items = table["first name"];
            Assert.That(items, Contains.Item("first item"));
            Assert.That(items, Contains.Item("second item"));
            Assert.That(items.Count(), Is.EqualTo(2));

            items = table["second name"];
            Assert.That(items, Contains.Item("third item"));
            Assert.That(items.Count(), Is.EqualTo(1));

            items = table["empty name"];
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void EmptyFileReturnsEmptyMapping()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                         </collections>";

            var table = mapper.Map(tableName);
            Assert.That(table, Is.Empty);
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
        public void WrongRootNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <wrongnode>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <collection>
                                 <name>second name</name>
                                 <entry>third item</entry>
                             </collection>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </wrongnode>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Table {tableName} is not a collection table (does not have \"collections\" as root node)"));
        }

        [Test]
        public void WrongCollectionNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <wrongnode>
                                 <name>second name</name>
                                 <entry>third item</entry>
                             </wrongnode>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </collections>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Collection table {tableName} contains an invalid collection node called \"wrongnode\""));
        }

        [Test]
        public void WrongInnerNodeThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <collection>
                                 <name>second name</name>
                                 <entry>third item</entry>
                                 <wrongnode>wrong item</wrongnode>
                             </collection>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </collections>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Collection table {tableName} has a malformed collection node"));
        }

        [Test]
        public void DuplicateNameNodesThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <collection>
                                 <name>second name</name>
                                 <name>wrong name</name>
                                 <entry>third item</entry>
                             </collection>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </collections>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Collection table {tableName} has a malformed collection node"));
        }

        [Test]
        public void DuplicateNamesThrowError()
        {
            contents = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                         <collections>
                             <collection>
                                 <name>first name</name>
                                 <entry>first item</entry>
                                 <entry>second item</entry>
                             </collection>
                             <collection>
                                 <name>first name</name>
                                 <entry>other item</entry>
                             </collection>
                             <collection>
                                 <name>second name</name>
                                 <entry>third item</entry>
                             </collection>
                             <collection>
                                 <name>empty name</name>
                             </collection>
                         </collections>";

            Assert.That(() => mapper.Map(tableName), Throws.Exception.With.Message.EqualTo($"Collection table {tableName} has duplicate collections of first name"));
        }
    }
}