using System.Linq;
using System.Text.Json;
using PluginSdk.Config;
using Xunit;

namespace PluginSdk.Tests
{
    /// <summary>
    /// Tests for the JSON envelope shape, the layout tree extraction and the
    /// per-option schema metadata used by the external manager app.
    /// </summary>
    public class SchemaTests
    {
        [Fact]
        public void Build_ExtractsLayoutContainersFromClassAttributes()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            // 2 tabs + 2 sections + 2 columns declared on TestConfig
            Assert.Equal(6, schema.Layout.Count);

            var general = schema.Layout.Single(c => c.Id == "general");
            Assert.Equal("tab", general.Kind);
            Assert.Null(general.Parent);
            Assert.Equal("General", general.Caption);

            var scalars = schema.Layout.Single(c => c.Id == "scalars");
            Assert.Equal("section", scalars.Kind);
            Assert.Equal("general", scalars.Parent);

            var left = schema.Layout.Single(c => c.Id == "scalars-left");
            Assert.Equal("column", left.Kind);
            Assert.Equal("scalars", left.Parent);
        }

        [Fact]
        public void Build_OptionsCarryParentReferences()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            var flag = schema.Properties.Single(p => p.Name == "Flag");
            Assert.Equal("bool", flag.Type);
            Assert.Equal("scalars-left", flag.Parent);

            var integer = schema.Properties.Single(p => p.Name == "Integer");
            Assert.Equal("int", integer.Type);
            Assert.Equal(0, integer.Min);
            Assert.Equal(100, integer.Max);
            Assert.Equal("scalars-left", integer.Parent);

            var longInteger = schema.Properties.Single(p => p.Name == "LongInteger");
            Assert.Equal("long", longInteger.Type);
            // No range set -> Min/Max should be null, not the sentinel
            Assert.Null(longInteger.Min);
            Assert.Null(longInteger.Max);
        }

        [Fact]
        public void Build_DescribesEachCollectionShape()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            var stringList = schema.Properties.Single(p => p.Name == "StringList");
            Assert.Equal("list", stringList.Type);
            Assert.Equal("string", stringList.ElementType);
            Assert.Null(stringList.ElementStruct);
            Assert.Null(stringList.TreeParentField);

            var dictLongBool = schema.Properties.Single(p => p.Name == "DictLongBool");
            Assert.Equal("dict", dictLongBool.Type);
            Assert.Equal("long", dictLongBool.KeyType);
            Assert.Equal("bool", dictLongBool.ValueType);

            var dictIntString = schema.Properties.Single(p => p.Name == "DictIntString");
            Assert.Equal("int", dictIntString.KeyType);
            Assert.Equal("string", dictIntString.ValueType);
        }

        [Fact]
        public void Build_StructPropertyReferencesStructDefinition()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            var structValue = schema.Properties.Single(p => p.Name == "StructValue");
            Assert.Equal("struct", structValue.Type);
            Assert.Equal(nameof(TestStruct), structValue.StructName);

            Assert.True(schema.Structs.ContainsKey(nameof(TestStruct)));
            var members = schema.Structs[nameof(TestStruct)];
            // 5 fields + 1 property
            Assert.Equal(6, members.Count);
            Assert.Contains(members, m => m.Name == "Flag" && m.Type == "bool");
            Assert.Contains(members, m => m.Name == "Text" && m.Type == "string");
        }

        [Fact]
        public void Build_ListOfStruct_CapturesElementStructName()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            var structList = schema.Properties.Single(p => p.Name == "StructList");
            Assert.Equal("list", structList.Type);
            Assert.Equal("struct", structList.ElementType);
            Assert.Equal(nameof(TestStruct), structList.ElementStruct);
        }

        [Fact]
        public void Build_TreeListExposesTreeParentField()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            var tree = schema.Properties.Single(p => p.Name == "TreeNodes");
            Assert.Equal("list", tree.Type);
            Assert.Equal("struct", tree.ElementType);
            Assert.Equal(nameof(TreeNode), tree.ElementStruct);
            Assert.Equal("ParentId", tree.TreeParentField);
            Assert.True(schema.Structs.ContainsKey(nameof(TreeNode)));
        }

        [Fact]
        public void Build_NestedStruct_DescribesCollectionAndStructMembers()
        {
            var schema = ConfigSchema.Build(typeof(TestConfig));

            Assert.True(schema.Structs.ContainsKey(nameof(NestedStruct)));
            var members = schema.Structs[nameof(NestedStruct)];

            var numbers = members.Single(m => m.Name == "Numbers");
            Assert.Equal("list", numbers.Type);
            Assert.Equal("int", numbers.ElementType);

            var map = members.Single(m => m.Name == "Map");
            Assert.Equal("dict", map.Type);
            Assert.Equal("string", map.KeyType);
            Assert.Equal("double", map.ValueType);

            var inner = members.Single(m => m.Name == "Inner");
            Assert.Equal("struct", inner.Type);
            Assert.Equal(nameof(TestStruct), inner.StructName);
        }

        [Fact]
        public void Build_NestedStruct_TransitivelyRegistersInnerStruct()
        {
            // NestedStruct references TestStruct as a member; even though
            // TestStruct is also referenced from top-level properties here,
            // the recursive struct walk must register it from inside the
            // nested type independently of top-level discovery.
            var schema = ConfigSchema.Build(typeof(TestConfig));
            Assert.True(schema.Structs.ContainsKey(nameof(NestedStruct)));
            Assert.True(schema.Structs.ContainsKey(nameof(TestStruct)));
        }
    }

    /// <summary>
    /// Tests that verify the JSON envelope produced by
    /// <see cref="ConfigStorage.SaveJson"/>: shape, presence of all options
    /// (defaults too), and a single round-trip via the wire format.
    /// </summary>
    public class JsonEnvelopeTests
    {
        [Fact]
        public void SaveJson_ProducesEnvelopeWithSchemaDefaultsAndValues()
        {
            var json = ConfigStorage.SaveJson(new TestConfig { Integer = 5 });
            using var doc = JsonDocument.Parse(json);

            Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
            Assert.True(doc.RootElement.TryGetProperty("schema", out _));
            Assert.True(doc.RootElement.TryGetProperty("defaults", out _));
            Assert.True(doc.RootElement.TryGetProperty("values", out _));
        }

        [Fact]
        public void SaveJson_IncludesAllOptionsEvenAtDefault()
        {
            var json = ConfigStorage.SaveJson(new TestConfig());
            using var doc = JsonDocument.Parse(json);
            var values = doc.RootElement.GetProperty("values");

            // PascalCase property names become camelCase in JSON.
            foreach (var name in new[] {
                "flag", "integer", "longInteger", "floatNumber", "doubleNumber", "text",
                "boolList", "intList", "longList", "floatList", "doubleList", "stringList",
                "dictStringInt", "dictStringString", "dictStringDouble",
                "dictIntString", "dictIntDouble", "dictLongBool", "dictLongLong",
                "structValue", "structList", "treeNodes", "nested",
            })
            {
                Assert.True(values.TryGetProperty(name, out _), $"values is missing '{name}'");
            }
        }

        [Fact]
        public void SaveJson_DefaultsSectionMirrorsFreshInstance()
        {
            var c = new TestConfig { Integer = 99, Text = "changed" };
            var json = ConfigStorage.SaveJson(c);
            using var doc = JsonDocument.Parse(json);

            var defaults = doc.RootElement.GetProperty("defaults");
            Assert.Equal(0, defaults.GetProperty("integer").GetInt32());
            Assert.Equal("", defaults.GetProperty("text").GetString());

            var values = doc.RootElement.GetProperty("values");
            Assert.Equal(99, values.GetProperty("integer").GetInt32());
            Assert.Equal("changed", values.GetProperty("text").GetString());
        }

        [Fact]
        public void SaveJson_SchemaMatchesConfigSchemaBuild()
        {
            var json = ConfigStorage.SaveJson(new TestConfig());
            using var doc = JsonDocument.Parse(json);

            var schemaEl = doc.RootElement.GetProperty("schema");
            Assert.Equal(6, schemaEl.GetProperty("layout").GetArrayLength());

            var properties = schemaEl.GetProperty("properties");
            Assert.True(properties.GetArrayLength() >= 20);

            // Spot-check: integer carries min/max
            var integerProp = properties.EnumerateArray().Single(p => p.GetProperty("name").GetString() == "Integer");
            Assert.Equal(0, integerProp.GetProperty("min").GetDouble());
            Assert.Equal(100, integerProp.GetProperty("max").GetDouble());
        }

        [Fact]
        public void LoadJson_AcceptsFlatValuesOnlyDocument()
        {
            // Backward-compat path: a flat values-only document (no envelope).
            var flatJson = "{\"flag\":true,\"integer\":7,\"text\":\"flat\"}";
            var loaded = ConfigStorage.LoadJson<TestConfig>(flatJson);

            Assert.True(loaded.Flag);
            Assert.Equal(7, loaded.Integer);
            Assert.Equal("flat", loaded.Text);
        }
    }

    /// <summary>
    /// Tests that verify the on-disk XML format is sparse: only properties
    /// whose value differs from the default are written, and loading a file
    /// that omits a property leaves it at the constructor's default.
    /// </summary>
    public class SparseXmlTests
    {
        [Fact]
        public void SaveXml_EmptyConfigWritesNoPropertyElements()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sparse-empty-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(new TestConfig(), path);
                var text = System.IO.File.ReadAllText(path);

                // Root element only, no child <Flag>, <Integer>, etc.
                Assert.DoesNotContain("<Flag>", text);
                Assert.DoesNotContain("<Integer>", text);
                Assert.DoesNotContain("<Text>", text);
                Assert.DoesNotContain("<IntList>", text);
                Assert.DoesNotContain("<DictStringInt>", text);
                Assert.DoesNotContain("<StructValue>", text);
                Assert.Contains("<TestConfig", text);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Fact]
        public void SaveXml_WritesOnlyChangedProperties()
        {
            var c = new TestConfig
            {
                Integer = 42,
                Text = "hello",
                IntList = new System.Collections.Generic.List<int> { 1, 2, 3 },
            };
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sparse-some-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(c, path);
                var text = System.IO.File.ReadAllText(path);

                Assert.Contains("<Integer>42</Integer>", text);
                Assert.Contains("<Text>hello</Text>", text);
                Assert.Contains("<IntList>", text);

                // Unchanged properties stay out.
                Assert.DoesNotContain("<Flag>", text);
                Assert.DoesNotContain("<LongInteger>", text);
                Assert.DoesNotContain("<BoolList>", text);
                Assert.DoesNotContain("<DictStringInt>", text);
                Assert.DoesNotContain("<StructValue>", text);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Fact]
        public void LoadXml_MissingPropertyKeepsConstructorDefault()
        {
            // Hand-crafted XML that only mentions Integer; other properties
            // must come from the parameterless ctor.
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sparse-load-{System.Guid.NewGuid():N}.xml");
            System.IO.File.WriteAllText(path,
                "<?xml version=\"1.0\"?>\n<TestConfig><Integer>123</Integer></TestConfig>");
            try
            {
                var loaded = ConfigStorage.LoadXml<TestConfig>(path);
                Assert.Equal(123, loaded.Integer);
                Assert.False(loaded.Flag);
                Assert.Equal("", loaded.Text);
                Assert.Empty(loaded.IntList);
                Assert.Empty(loaded.DictStringInt);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }

        [Fact]
        public void SaveXml_StructAtDefaultIsOmitted_BothFieldAndContents()
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"sparse-struct-{System.Guid.NewGuid():N}.xml");
            try
            {
                ConfigStorage.SaveXml(new TestConfig { StructValue = new TestStruct { Integer = 7 } }, path);
                var text = System.IO.File.ReadAllText(path);
                Assert.Contains("<StructValue>", text);
                Assert.Contains("<Integer>7</Integer>", text);
            }
            finally
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
        }
    }
}
