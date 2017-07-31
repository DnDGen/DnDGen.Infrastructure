using DnDGen.Core.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace DnDGen.Core.Tests.Tables
{
    [TestFixture]
    public class EmbeddedResourceStreamLoaderTests
    {
        private StreamLoader streamLoader;
        private Mock<AssemblyLoader> mockAssemblyLoader;

        [SetUp]
        public void Setup()
        {
            mockAssemblyLoader = new Mock<AssemblyLoader>();
            streamLoader = new EmbeddedResourceStreamLoader(mockAssemblyLoader.Object);

            var assembly = Assembly.GetExecutingAssembly();
            mockAssemblyLoader.Setup(l => l.GetRunningAssembly()).Returns(assembly);
        }

        [Test]
        public void GetsFileIfItIsAnEmbeddedResource()
        {
            var table = new Dictionary<int, string>();
            var xmlDocument = new XmlDocument();

            using (var stream = streamLoader.LoadFor("TestTable.xml"))
                xmlDocument.Load(stream);

            var objects = xmlDocument.DocumentElement.ChildNodes;
            Assert.That(objects[0].SelectSingleNode("value").InnerText, Is.EqualTo("Value 1"));
            Assert.That(objects[1].SelectSingleNode("value").InnerText, Is.EqualTo("Value 2"));
            Assert.That(objects[0].ChildNodes.Count, Is.EqualTo(1));
            Assert.That(objects[1].ChildNodes.Count, Is.EqualTo(3));
            Assert.That(objects.Count, Is.EqualTo(2));

            var subvalues = objects[1].SelectNodes("subvalue");
            Assert.That(subvalues[0].InnerText, Is.EqualTo("Subvalue 1"));
            Assert.That(subvalues[1].InnerText, Is.EqualTo("Subvalue 2"));
            Assert.That(subvalues.Count, Is.EqualTo(2));
        }

        [Test]
        public void ThrowErrorIfFileIsNotFormattedCorrectly()
        {
            Assert.That(() => streamLoader.LoadFor("TestTable"), Throws.ArgumentException.With.Message.EqualTo("\"TestTable\" is not a valid XML file"));
        }

        [Test]
        public void ThrowErrorIfFileIsNotXML()
        {
            Assert.That(() => streamLoader.LoadFor("TestTable.pdf"), Throws.ArgumentException.With.Message.EqualTo("\"TestTable.pdf\" is not a valid XML file"));
        }

        [Test]
        public void ThrowErrorIfFileIsNotAnEmbeddedResource()
        {
            Assert.That(() => streamLoader.LoadFor("invalid filename.xml"), Throws.InstanceOf<FileNotFoundException>().With.Message.EqualTo("invalid filename.xml"));
        }

        [Test]
        public void MatchWholeFileName()
        {
            Assert.That(() => streamLoader.LoadFor("Table.xml"), Throws.InstanceOf<FileNotFoundException>().With.Message.EqualTo("Table.xml"));
        }
    }
}
