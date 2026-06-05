# PluginSdkTests/SerializationTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test classes · **Lines:** 464

## Summary
End-to-end round-trip and format-pinning tests for `PluginSdk.Config` serialisation. The file contains three test classes covering every type combination the library must support: `SerializationTests` for XML/JSON round-trips of all scalar, enum, list, dict, struct, nested-struct, and VRage-type properties; `TypeSerializationTests` for the exact on-disk and on-wire representation of VRage value types (`Color`, `Vector2D`, `Vector3D`, `Vector2I`, `Vector3I`, `Base6Directions.Direction`, `MyPositionAndOrientation`); and helpers that are shared across tests. These tests are the definitive contract for what `ConfigStorage` must produce and accept.

## Types

### `SerializationTests` — class, public

XML and JSON round-trip tests for the complete `TestConfig` fixture. Uses a shared factory method `MakePopulatedConfig` that sets every property to a non-default value and a shared `AssertEqual` deep-comparison helper.

- **Methods:**
  - `MakePopulatedConfig() → TestConfig` (private static) — constructs a fully-populated `TestConfig` with non-default values for all scalars, all list types (`bool`, `int`, `long`, `float`, `double`, `string`), all dict combinations (`string`→`int`/`string`/`double`, `int`→`string`/`double`, `long`→`bool`/`long`), `Quality` enum and `List<Quality>`, `TestStruct` (with all fields + `Quality.Medium`), `List<TestStruct>`, `List<TreeNode>`, `NestedStruct` (with nested list/dict/inner struct), and all VRage types (`Color` ×2, `Vector2D`, `Vector3D`, `Vector2I`, `Vector3I`, `Base6Directions.Direction`, `MyPositionAndOrientation`).
  - `AssertEqual(TestConfig, TestConfig)` (private static) — field-by-field deep-equality assertions covering all property groups; used by round-trip tests.
  - `Xml_RoundTrip_PreservesAllValues` — saves via `ConfigStorage.SaveXml`, reloads via `ConfigStorage.LoadXml<TestConfig>`, calls `AssertEqual`.
  - `Xml_LoadingMissingFile_ReturnsDefaultInstance` — absent file path returns `new TestConfig()` (non-null, all defaults).
  - `Xml_SaveWritesAtomicallyViaTempFile` — after save the target file exists but `path + ".tmp"` does not, confirming atomic-rename behaviour.
  - `Json_RoundTrip_PreservesAllValues` — saves via `ConfigStorage.SaveJson`, reloads via `ConfigStorage.LoadJson<TestConfig>`, calls `AssertEqual`.
  - `Json_RoundTrip_EmptyConfig_PreservesDefaults` — default `TestConfig` survives JSON round-trip intact.
  - `Xml_EnumValueIsStoredByMemberName` — `Quality.High` (underlying value 10) is written as `<Quality>High</Quality>`, not `<Quality>10</Quality>`.

---

### `TypeSerializationTests` — class, public

Pins the exact on-disk XML and on-wire JSON format of VRage value types, which bypass the generic serialisers and therefore have their format as an explicit contract.

- **Methods:**
  - `WriteXml(TestConfig) → string` (private static) — saves to a temp file and returns the raw text; deletes the file in `finally`.
  - `Xml_Color_UsesHexRgbaFormat` — `Color(0xAB, 0xCD, 0xEF, 0xFF)` → `<SolidColor>#ABCDEFFF</SolidColor>`.
  - `Xml_Vectors_UseSpaceSeparatedComponents` — `Vector2D`, `Vector3D` use G17 formatting with space-separated components; `Vector2I`, `Vector3I` use space-separated integers; verified with regex assertions.
  - `Xml_Direction_StoredByMemberName` — `Base6Directions.Direction.Backward` → `<Facing>Backward</Facing>`.
  - `Xml_PositionAndOrientation_HasNestedPositionForwardUpOnly` — `<SpawnPose>` contains `<Position>`, `<Forward>`, `<Up>` elements but no `<Orientation>` quaternion element.
  - `Xml_DefaultValueIsOmitted` — a `SpawnPose` equal to the constructor default produces no `<SpawnPose>` element.
  - `Xml_Color_AcceptsBothRgbAndRgbaInputs` — loads hand-crafted XML: 6-digit hex (`#102030`) defaults alpha to 255; 8-digit hex (`#10203040`) takes the explicit alpha.
  - `Json_Color_UsesHexString` — `Color(1,2,3,255)` serialises as `"#010203FF"` string in the `values` section.
  - `Json_Vector3D_UsesObjectShape` — `Vector3D(1.5, 2.5, 3.5)` → `{"x":1.5, "y":2.5, "z":3.5}` object.
  - `Json_Direction_IsStringMemberName` — `Direction.Left` → `"Left"` string.
  - `Json_Pose_HasOnlyPositionForwardUp` — JSON pose object has `position`, `forward`, `up` properties but not `orientation`.
  - `Schema_ColorOption_ExposesHasAlphaFlag` — `SolidColor` (RGB) → `HasAlpha == false`; `TintColor` (RGBA) → `HasAlpha == true`.
  - `Schema_VectorAndPoseAndDirection_HaveDistinctTypeNames` — each VRage vector/pose/direction property carries the exact C# type name as its schema type token; `Direction` is also registered as an enum in `schema.Enums`.

## Cross-references
- **Uses:**
  - `PluginSdkTests/TestConfig.cs` — `TestConfig`, `TestStruct`, `TreeNode`, `NestedStruct`, `Quality`
  - `PluginSdk/Config/` — `ConfigStorage`, `ConfigSchema`
  - `PluginSdk/Tools/SerializableDictionary.cs` — `SerializableDictionary<TK,TV>`
  - SE DS / VRage assemblies — `VRage.Color`, `VRageMath.Vector2D`, `VRageMath.Vector3D`, `VRageMath.Vector2I`, `VRageMath.Vector3I`, `VRageMath.Base6Directions.Direction`, `VRage.MyPositionAndOrientation`
- **Used by:** _none within the repository_
