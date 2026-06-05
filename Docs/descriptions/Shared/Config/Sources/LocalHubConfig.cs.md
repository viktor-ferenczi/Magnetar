# Shared/Config/Sources/LocalHubConfig.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Config` · **Kind:** class · **Lines:** 9

## Summary
`LocalHubConfig` is the configuration record for a locally-stored plugin hub — a directory on the filesystem that acts as a hub catalogue. It is stored as an `<LocalHub>` element inside `SourcesConfig` and gives the loader the folder path and an optional content hash to detect when the hub index has changed.

## Types

### LocalHubConfig — class, public

Plain data-transfer object representing one local hub source. The `Hash` field allows the loader to skip re-scanning the folder when its content has not changed since the last run.

- **Properties:** `Name` — display name for the hub; `Folder` — filesystem path to the hub directory; `Enabled` — whether this hub is active; `Hash` — last-known content hash used to detect hub updates.

## Cross-references
- **Uses:** _(none — pure DTO)_
- **Used by:** [SourcesConfig.cs](../SourcesConfig.cs.md), [PluginList.cs](../../PluginList.cs.md)
