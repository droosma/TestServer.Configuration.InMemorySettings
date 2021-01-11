using System.Collections.Generic;

using FluentAssertions;

using Xunit;

namespace TestServer.Configuration.InMemorySettings.Tests.Unit
{
    public class UnitTest1
    {
        [Fact]
        public void AsInMemoryCollection_GivenClassWithEnumerableChild_GeneratesCorrectKeyValuePair()
        {
            const string itemValue = "valueOne";
            var sut = new TestSettingWithCollection {Values = new List<string> {itemValue}};

            var result = sut.AsInMemoryCollection();

            result.Should().Contain(pair => pair.Key == "Values:0"
                                            && pair.Value == itemValue);
        }

        [Fact]
        public void AsInMemoryCollection_GivenClassWithNestedChildren_GeneratesCorrectKeyValuePair()
        {
            const string settingValue = "setting-name";
            var sut = new TestSettingWithNesting {NestedTestSetting = new TestSetting {Name = settingValue}};

            var result = sut.AsInMemoryCollection();

            result.Should().Contain(pair => pair.Key == $"{nameof(TestSettingWithNesting.NestedTestSetting)}:{nameof(TestSetting.Name)}"
                                            && pair.Value == settingValue);
        }

        [Fact]
        public void AsInMemoryCollection_GivenSimpleTypeChild_GeneratesCorrectKeyValuePair()
        {
            const string settingValue = "setting-name";
            var sut = new TestSetting {Name = settingValue};

            var result = sut.AsInMemoryCollection();

            result.Should().Contain(pair => pair.Key == nameof(TestSetting.Name)
                                            && pair.Value == settingValue);
        }

        private class TestSetting
        {
            public string Name { get; set; }
        }

        private class TestSettingWithNesting
        {
            public TestSetting NestedTestSetting { get; set; }
        }

        private class TestSettingWithCollection
        {
            public List<string> Values { get; set; }
        }
    }
}