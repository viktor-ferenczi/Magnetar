# Shared/Data/ObsoletePlugin.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Data` · **Kind:** class (`PluginData` subclass) · **Lines:** 15

## Summary
`ObsoletePlugin` is a placeholder `PluginData` registered as a ProtoBuf subtype so the plugin-list deserializer can tolerate plugins that have been removed or superseded. It carries no behavior and loads no assembly — it exists purely to keep `[ProtoInclude(100, ...)]` slots stable and to mark such entries as obsolete in the list.

## Types
### ObsoletePlugin — class, internal : `PluginData`
A no-op plugin entry. Hides `Source` with the constant `"Obsolete"`. Registered via `[ProtoInclude(100, typeof(ObsoletePlugin))]` on `PluginData`.
- **Properties:** `Source` (`new`) => `"Obsolete"`; `IsLocal` => `false`; `IsCompiled` => `false`.
- **Methods:** `GetAssembly()` => `null`.

## Cross-references
- **Uses:** `PluginData` (Shared/Data/PluginData.cs); ProtoBuf (`ProtoInclude` registration on `PluginData`).
- **Used by:** [PluginData.cs](PluginData.cs.md), [PluginList.cs](../PluginList.cs.md)
