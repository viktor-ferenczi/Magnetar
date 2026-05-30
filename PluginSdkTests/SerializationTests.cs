using System.Collections.Generic;
using System.IO;
using PluginSdk.Config;
using PluginSdk.Tools;
using Xunit;

namespace PluginSdk.Tests
{
    /// <summary>
    /// XML and JSON round-trip tests covering every type combination
    /// supported by <see cref="PluginConfig"/>: scalars, enums, lists of
    /// scalars and enums, dictionaries with each allowed key type, structs
    /// (including with enum members), and lists of structs.
    /// </summary>
    public class SerializationTests
    {
        private static TestConfig MakePopulatedConfig()
        {
            return new TestConfig
            {
                Flag = true,
                Integer = 42,
                LongInteger = 1L << 40,
                FloatNumber = 1.5f,
                DoubleNumber = 2.71828,
                Text = "hello",

                BoolList = new List<bool> { true, false, true },
                IntList = new List<int> { -1, 0, 1, 2, 3 },
                LongList = new List<long> { -9_000_000_000L, 9_000_000_000L },
                FloatList = new List<float> { 0.1f, 0.2f },
                DoubleList = new List<double> { 1.1, 2.2, 3.3 },
                StringList = new List<string> { "a", "b", "c" },

                DictStringInt = new SerializableDictionary<string, int>
                {
                    ["one"] = 1,
                    ["two"] = 2,
                },
                DictStringString = new SerializableDictionary<string, string>
                {
                    ["greeting"] = "hello",
                },
                DictStringDouble = new SerializableDictionary<string, double>
                {
                    ["pi"] = 3.14,
                    ["e"] = 2.72,
                },
                DictIntString = new SerializableDictionary<int, string>
                {
                    [1] = "one",
                    [2] = "two",
                },
                DictIntDouble = new SerializableDictionary<int, double>
                {
                    [10] = 10.5,
                },
                DictLongBool = new SerializableDictionary<long, bool>
                {
                    [1L << 33] = true,
                    [1L << 34] = false,
                },
                DictLongLong = new SerializableDictionary<long, long>
                {
                    [100L] = 200L,
                },

                Quality = Quality.High,
                QualityList = new List<Quality> { Quality.Low, Quality.High, Quality.Medium },

                StructValue = new TestStruct
                {
                    Flag = true,
                    Integer = 7,
                    LongInteger = 1L << 35,
                    FloatNumber = 0.5f,
                    DoubleNumber = 1.5,
                    Text = "nested",
                    Quality = Quality.Medium,
                },

                StructList = new List<TestStruct>
                {
                    new TestStruct { Flag = true,  Integer = 1, Text = "first"  },
                    new TestStruct { Flag = false, Integer = 2, Text = "second" },
                },

                TreeNodes = new List<TreeNode>
                {
                    new TreeNode { Id = 1, ParentId = 0, Label = "root"   },
                    new TreeNode { Id = 2, ParentId = 1, Label = "child"  },
                    new TreeNode { Id = 3, ParentId = 1, Label = "child2" },
                },

                Nested = new NestedStruct
                {
                    Name = "outer",
                    Numbers = new List<int> { 10, 20, 30 },
                    Map = new SerializableDictionary<string, double> { ["k"] = 1.5 },
                    Inner = new TestStruct { Integer = 9, Text = "inner" },
                },
            };
        }

        private static void AssertEqual(TestConfig expected, TestConfig actual)
        {
            Assert.Equal(expected.Flag, actual.Flag);
            Assert.Equal(expected.Integer, actual.Integer);
            Assert.Equal(expected.LongInteger, actual.LongInteger);
            Assert.Equal(expected.FloatNumber, actual.FloatNumber);
            Assert.Equal(expected.DoubleNumber, actual.DoubleNumber);
            Assert.Equal(expected.Text, actual.Text);

            Assert.Equal(expected.BoolList, actual.BoolList);
            Assert.Equal(expected.IntList, actual.IntList);
            Assert.Equal(expected.LongList, actual.LongList);
            Assert.Equal(expected.FloatList, actual.FloatList);
            Assert.Equal(expected.DoubleList, actual.DoubleList);
            Assert.Equal(expected.StringList, actual.StringList);

            Assert.Equal(expected.DictStringInt, actual.DictStringInt);
            Assert.Equal(expected.DictStringString, actual.DictStringString);
            Assert.Equal(expected.DictStringDouble, actual.DictStringDouble);
            Assert.Equal(expected.DictIntString, actual.DictIntString);
            Assert.Equal(expected.DictIntDouble, actual.DictIntDouble);
            Assert.Equal(expected.DictLongBool, actual.DictLongBool);
            Assert.Equal(expected.DictLongLong, actual.DictLongLong);

            Assert.Equal(expected.Quality, actual.Quality);
            Assert.Equal(expected.QualityList, actual.QualityList);

            Assert.Equal(expected.StructValue.Flag, actual.StructValue.Flag);
            Assert.Equal(expected.StructValue.Integer, actual.StructValue.Integer);
            Assert.Equal(expected.StructValue.LongInteger, actual.StructValue.LongInteger);
            Assert.Equal(expected.StructValue.FloatNumber, actual.StructValue.FloatNumber);
            Assert.Equal(expected.StructValue.DoubleNumber, actual.StructValue.DoubleNumber);
            Assert.Equal(expected.StructValue.Text, actual.StructValue.Text);
            Assert.Equal(expected.StructValue.Quality, actual.StructValue.Quality);

            Assert.Equal(expected.StructList.Count, actual.StructList.Count);
            for (int i = 0; i < expected.StructList.Count; i++)
            {
                Assert.Equal(expected.StructList[i].Flag, actual.StructList[i].Flag);
                Assert.Equal(expected.StructList[i].Integer, actual.StructList[i].Integer);
                Assert.Equal(expected.StructList[i].LongInteger, actual.StructList[i].LongInteger);
                Assert.Equal(expected.StructList[i].FloatNumber, actual.StructList[i].FloatNumber);
                Assert.Equal(expected.StructList[i].DoubleNumber, actual.StructList[i].DoubleNumber);
                Assert.Equal(expected.StructList[i].Text, actual.StructList[i].Text);
            }

            Assert.Equal(expected.TreeNodes.Count, actual.TreeNodes.Count);
            for (int i = 0; i < expected.TreeNodes.Count; i++)
            {
                Assert.Equal(expected.TreeNodes[i].Id, actual.TreeNodes[i].Id);
                Assert.Equal(expected.TreeNodes[i].ParentId, actual.TreeNodes[i].ParentId);
                Assert.Equal(expected.TreeNodes[i].Label, actual.TreeNodes[i].Label);
            }

            Assert.Equal(expected.Nested.Name, actual.Nested.Name);
            Assert.Equal(expected.Nested.Numbers, actual.Nested.Numbers);
            Assert.Equal(expected.Nested.Map, actual.Nested.Map);
            Assert.Equal(expected.Nested.Inner.Integer, actual.Nested.Inner.Integer);
            Assert.Equal(expected.Nested.Inner.Text, actual.Nested.Inner.Text);
        }

        [Fact]
        public void Xml_RoundTrip_PreservesAllValues()
        {
            var original = MakePopulatedConfig();
            var path = Path.Combine(Path.GetTempPath(), $"magnetar-xml-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(original, path);
                Assert.True(File.Exists(path));
                var loaded = ConfigStorage.LoadXml<TestConfig>(path);
                AssertEqual(original, loaded);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void Xml_LoadingMissingFile_ReturnsDefaultInstance()
        {
            var path = Path.Combine(Path.GetTempPath(), $"magnetar-missing-{System.Guid.NewGuid():N}.xml");
            var loaded = ConfigStorage.LoadXml<TestConfig>(path);
            Assert.NotNull(loaded);
            Assert.False(loaded.Flag);
            Assert.Empty(loaded.IntList);
        }

        [Fact]
        public void Xml_SaveWritesAtomicallyViaTempFile()
        {
            var path = Path.Combine(Path.GetTempPath(), $"magnetar-atomic-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(new TestConfig(), path);
                Assert.True(File.Exists(path));
                Assert.False(File.Exists(path + ".tmp"));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void Json_RoundTrip_PreservesAllValues()
        {
            var original = MakePopulatedConfig();
            var json = ConfigStorage.SaveJson(original);
            Assert.False(string.IsNullOrWhiteSpace(json));
            var loaded = ConfigStorage.LoadJson<TestConfig>(json);
            AssertEqual(original, loaded);
        }

        [Fact]
        public void Json_RoundTrip_EmptyConfig_PreservesDefaults()
        {
            var original = new TestConfig();
            var json = ConfigStorage.SaveJson(original);
            var loaded = ConfigStorage.LoadJson<TestConfig>(json);
            AssertEqual(original, loaded);
        }

        [Fact]
        public void Xml_EnumValueIsStoredByMemberName()
        {
            // Quality.High has underlying value 10; storing it by name protects
            // against silent breakage if the enum is later renumbered.
            var c = new TestConfig { Quality = Quality.High };
            var path = Path.Combine(Path.GetTempPath(), $"magnetar-enum-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(c, path);
                var text = File.ReadAllText(path);
                Assert.Contains("<Quality>High</Quality>", text);
                Assert.DoesNotContain("<Quality>10</Quality>", text);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
