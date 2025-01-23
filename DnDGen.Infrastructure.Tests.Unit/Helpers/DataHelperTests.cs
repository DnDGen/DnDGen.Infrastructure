using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Tests.Unit.Models;
using NUnit.Framework;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Helpers
{
    [TestFixture]
    internal class DataHelperTests
    {
        [Test]
        public void Parse_FromNoSections_ReturnsString()
        {
            var rawData = DataHelper.Parse([]);
            Assert.That(rawData, Is.Empty);
        }

        [Test]
        public void Parse_FromSection_ReturnsString()
        {
            var rawData = DataHelper.Parse(["this is my section"]);
            Assert.That(rawData, Is.EqualTo("this is my section"));
        }

        [Test]
        public void Parse_FromSections_ReturnsString()
        {
            var rawData = DataHelper.Parse(["these", "are", "my", "sections, with commas"]);
            Assert.That(rawData, Is.EqualTo("these@are@my@sections, with commas"));
        }

        [Test]
        public void Parse_FromSections_ReturnsString_OverrideSeparator()
        {
            var rawData = DataHelper.Parse(["these", "are", "my", "sections@gmail"], ',');
            Assert.That(rawData, Is.EqualTo("these,are,my,sections@gmail"));
        }

        [Test]
        public void Parse_FromEmptyString_ReturnsEmptySection()
        {
            var rawData = DataHelper.Parse(string.Empty);
            Assert.That(rawData, Is.EqualTo([string.Empty]));
        }

        [Test]
        public void Parse_FromString_ReturnsSection()
        {
            var rawData = DataHelper.Parse("this is my section");
            Assert.That(rawData, Is.EqualTo(["this is my section"]));
        }

        [Test]
        public void Parse_FromString_ReturnsSections()
        {
            var rawData = DataHelper.Parse("these@are@my@sections, with commas");
            Assert.That(rawData, Is.EqualTo(["these", "are", "my", "sections, with commas"]));
        }

        [Test]
        public void Parse_FromString_ReturnsSections_OverrideSeparator()
        {
            var rawData = DataHelper.Parse("these,are,my,sections@gmail", ',');
            Assert.That(rawData, Is.EqualTo(["these", "are", "my", "sections@gmail"]));
        }

        [TestCase("my name@9266", "my name", 9266)]
        [TestCase("My Name@90210", "My Name", 90210)]
        [TestCase("@42", "", 42)]
        [TestCase("@0", "", 0)]
        public void Parse_FromRawData_ImplicitMap_ReturnsSelection(string data, string name, int age)
        {
            var selection = DataHelper.Parse<FakeDataSelection>(data);
            Assert.That(selection.Name, Is.EqualTo(name));
            Assert.That(selection.Age, Is.EqualTo(age));
        }

        [Test]
        public void Parse_FromRawData_ImplicitMap_ThrowsArgumentException_WhenTooFewSections()
        {
            Assert.That(() => DataHelper.Parse<FakeDataSelection>("bad data"),
                Throws.ArgumentException.With.Message.EqualTo("Data [bad data] invalid for DnDGen.Infrastructure.Tests.Unit.Models.FakeDataSelection: Need 2 sections, got 1"));
        }

        [Test]
        public void Parse_FromRawData_ImplicitMap_ThrowsArgumentException_WhenTooManySections()
        {
            Assert.That(() => DataHelper.Parse<FakeDataSelection>("bad@data@"),
                Throws.ArgumentException.With.Message.EqualTo("Data [bad@data@] invalid for DnDGen.Infrastructure.Tests.Unit.Models.FakeDataSelection: Need 2 sections, got 3"));
        }

        [Test]
        public void Parse_FromRawData_ImplicitMap_ReturnsSelection_OverriddenSeparator()
        {
            var selection = DataHelper.Parse<OtherFakeDataSelection>("my string,600,true");
            Assert.That(selection.MyString, Is.EqualTo("my string"));
            Assert.That(selection.MyInt, Is.EqualTo(600));
            Assert.That(selection.MyBoolean, Is.True);
        }

        [TestCase("9266@my name", "my name", 9266)]
        [TestCase("90210@My Name", "My Name", 90210)]
        [TestCase("42@", "", 42)]
        [TestCase("0@", "", 0)]
        public void Parse_FromRawData_ExplicitMap_ReturnsSelection(string data, string something, int whatever)
        {
            var selection = DataHelper.Parse(data, MapBadData);
            Assert.That(selection.Something, Is.EqualTo(something));
            Assert.That(selection.Whatever, Is.EqualTo(whatever));
        }

        [Test]
        public void Parse_FromRawData_ExplicitMap_ThrowsArgumentException_WhenTooFewSections()
        {
            Assert.That(() => DataHelper.Parse("bad data", MapBadData),
                Throws.ArgumentException.With.Message.EqualTo("Data [bad data] invalid for DnDGen.Infrastructure.Tests.Unit.Models.BadDataSelection: Need 2 sections, got 1"));
        }

        [Test]
        public void Parse_FromRawData_ExplicitMap_ThrowsArgumentException_WhenTooManySections()
        {
            Assert.That(() => DataHelper.Parse("bad@data@", MapBadData),
                Throws.ArgumentException.With.Message.EqualTo("Data [bad@data@] invalid for DnDGen.Infrastructure.Tests.Unit.Models.BadDataSelection: Need 2 sections, got 3"));
        }

        [Test]
        public void Parse_FromRawData_ExplicitMap_ReturnsSelection_OverriddenSeparator()
        {
            var selection = DataHelper.Parse("600,my string", MapBadData, ',');
            Assert.That(selection.Something, Is.EqualTo("my string"));
            Assert.That(selection.Whatever, Is.EqualTo(600));
        }

        private BadDataSelection MapBadData(string[] data) => new()
        {
            Whatever = Convert.ToInt32(data[0]),
            Something = data[1]
        };

        [TestCase("my name@9266", "my name", 9266)]
        [TestCase("My Name@90210", "My Name", 90210)]
        [TestCase("@42", "", 42)]
        [TestCase("@0", "", 0)]
        public void Parse_FromSelection_ImplicitMap_ReturnsRawData(string data, string name, int age)
        {
            var selection = new FakeDataSelection { Name = name, Age = age };
            var rawData = DataHelper.Parse(selection);
            Assert.That(rawData, Is.EqualTo(data));
        }

        [Test]
        public void Parse_FromSelection_ImplicitMap_ThrowsArgumentException_WhenCountTooHigh()
        {
            var selection = new OtherBadDataSelection { FirstName = "my first name", LastName = "my last name", ManualSectionCount = 3 };
            Assert.That(() => DataHelper.Parse(selection),
                Throws.ArgumentException.With.Message.EqualTo("Data [my first name@my last name] invalid for DnDGen.Infrastructure.Tests.Unit.Models.OtherBadDataSelection: Need 3 sections, got 2"));
        }

        [Test]
        public void Parse_FromSelection_ImplicitMap_ThrowsArgumentException_WhenCountTooLow()
        {
            var selection = new OtherBadDataSelection { FirstName = "my first name", LastName = "my last name", ManualSectionCount = 1 };
            Assert.That(() => DataHelper.Parse(selection),
                Throws.ArgumentException.With.Message.EqualTo("Data [my first name@my last name] invalid for DnDGen.Infrastructure.Tests.Unit.Models.OtherBadDataSelection: Need 1 sections, got 2"));
        }

        [Test]
        public void Parse_FromSelection_ImplicitMap_ReturnsRawData_OverriddenSeparator()
        {
            var selection = new OtherFakeDataSelection { MyString = "my string", MyInt = 600, MyBoolean = true };
            var rawData = DataHelper.Parse(selection);
            Assert.That(rawData, Is.EqualTo("my string,600,True"));
        }

        [TestCase("9266@my name", "my name", 9266)]
        [TestCase("90210@My Name", "My Name", 90210)]
        [TestCase("42@", "", 42)]
        [TestCase("0@", "", 0)]
        public void Parse_FromSelection_ExplicitMap_ReturnsRawData(string data, string something, int whatever)
        {
            var selection = new BadDataSelection { Whatever = whatever, Something = something };
            var rawData = DataHelper.Parse(selection, MapBadData);
            Assert.That(rawData, Is.EqualTo(data));
        }

        [Test]
        public void Parse_FromSelection_ExplicitMap_ThrowsArgumentException_WhenCountTooHigh()
        {
            var selection = new OtherBadDataSelection { FirstName = "my first name", LastName = "my last name", ManualSectionCount = 3 };
            Assert.That(() => DataHelper.Parse(selection, MapOtherBadData),
                Throws.ArgumentException.With.Message.EqualTo("Data [my first name@my last name] invalid for DnDGen.Infrastructure.Tests.Unit.Models.OtherBadDataSelection: Need 3 sections, got 2"));
        }

        [Test]
        public void Parse_FromSelection_ExplicitMap_ThrowsArgumentException_WhenCountTooLow()
        {
            var selection = new OtherBadDataSelection { FirstName = "my first name", LastName = "my last name", ManualSectionCount = 1 };
            Assert.That(() => DataHelper.Parse(selection, MapOtherBadData),
                Throws.ArgumentException.With.Message.EqualTo("Data [my first name@my last name] invalid for DnDGen.Infrastructure.Tests.Unit.Models.OtherBadDataSelection: Need 1 sections, got 2"));
        }

        [Test]
        public void Parse_FromSelection_ExplicitMap_ReturnsRawData_OverriddenSeparator()
        {
            var selection = new BadDataSelection { Whatever = 600, Something = "my string" };
            var rawData = DataHelper.Parse(selection, MapBadData, ',');
            Assert.That(rawData, Is.EqualTo("600,my string"));
        }

        private string[] MapBadData(BadDataSelection data) => [data.Whatever.ToString(), data.Something];
        private string[] MapOtherBadData(OtherBadDataSelection data) => [data.FirstName, data.LastName];
    }
}
