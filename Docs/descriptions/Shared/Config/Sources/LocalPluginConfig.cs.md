# Shared/Config/Sources/LocalPluginConfig.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Config` · **Kind:** class · **Lines:** 8

## Summary
`LocalPluginConfig` is the configuration record for a plugin that is installed directly from a local filesystem folder, without going through a hub or GitHub. It is serialised as a `<LocalPlugin>` element inside `SourcesConfig`, listing the folder path and enabling/disabling the plugin at the source level independently of any profile.

## Types

### LocalPluginConfig — class, public

Minimal data-transfer object for a directly-registered local plugin source.

- **Properties:** `Name` — display name for the plugin; `Folder` — path to the local plugin folder; `Enabled` — whether this plugin source is active.

## Cross-references
- **Uses:** _(none — pure DTO)_
- **Used by:** [SourcesConfig.cs](../SourcesConfig.cs.md), [PluginList.cs](../../PluginList.cs.md)
