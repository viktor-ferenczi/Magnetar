# Shared/Config/Sources/RemotePluginConfig.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Config` · **Kind:** class · **Lines:** 14

## Summary
`RemotePluginConfig` is the configuration record for a GitHub-hosted plugin that is registered directly as a source (not via a hub). It is serialised as a `<RemotePlugin>` element inside `SourcesConfig`. The `File` property identifies a specific asset or manifest file within the GitHub release, and `LastCheck` enables staleness-based cache invalidation identical to `RemoteHubConfig`.

## Types

### RemotePluginConfig — class, public

Data-transfer object for one directly-registered remote plugin source. Unlike `RemoteHubConfig`, this entry points at a specific plugin repository rather than a hub index.

- **Properties:** `Name` — display name for the plugin; `Repo` — GitHub repository identifier (e.g. `"owner/repo"`); `Branch` — branch to check for releases or raw file access; `File` — specific file name within the release assets to download; `LastCheck` — nullable `DateTime` of the last successful check, used with `SourcesConfig.MaxSourceAge`; `Enabled` — whether this source is active; `Trusted` — whether the plugin is loaded without an additional confirmation prompt.

## Cross-references
- **Uses:** _(none — pure DTO)_
- **Used by:** [SourcesConfig.cs](../SourcesConfig.cs.md), [PluginList.cs](../../PluginList.cs.md)
