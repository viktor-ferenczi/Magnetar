# Shared/Config/SourcesConfig.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Config` · **Kind:** class · **Lines:** 134

## Summary
`SourcesConfig` is the XML-serialised registry of all plugin and mod sources available to Magnetar. It is persisted to `<pulsarDir>/Sources/sources.xml` and holds five heterogeneous source collections: local hubs, remote (GitHub) hubs, remote plugins, local plugins, and Steam Workshop mods. It also carries global policy settings such as the staleness threshold for remote checks and whether to show the trusted-source warning. When the file does not exist it is initialised with a caller-supplied array of default remote hubs (typically the official Magnetar hub).

## Types

### SourcesConfig — class, public

Stateful, serialisable container for all registered sources. Each of the five source collections is backed by a private `HashSet<T>` (for deduplication) but exposed via an array-typed property with `[XmlArray]` / `[XmlArrayItem]` attributes so that `XmlSerializer` can round-trip it. The setter of each array property replaces the backing set by clearing and re-adding, preserving deduplication on deserialisation.

- **Fields:** `fileName` — constant file name (`"sources.xml"`); `filePath` — private path resolved after load; `localHubSources`, `remoteHubSources`, `remotePluginSources`, `localPluginSources`, `modSources` — private `HashSet<T>` backing stores for each source category.
- **Properties:** `ShowWarning` — whether to display the untrusted-source warning dialog (default `true`); `MaxSourceAge` — number of days before a remote source's cached index is considered stale and re-fetched (default `2`); `LocalHubSources` — `[XmlArray("LocalHubSources")]` array of `LocalHubConfig`; `RemoteHubSources` — array of `RemoteHubConfig`; `RemotePluginSources` — array of `RemotePluginConfig`; `LocalPluginSources` — array of `LocalPluginConfig`; `ModSources` — array of `ModConfig`.
- **Methods:** `Save()` — serialises the instance to `filePath` via `XmlSerializer`, creating the directory if needed; `Load(mainDirectory, defaultHubs)` — static factory that deserialises from `<mainDirectory>/Sources/sources.xml`; if the file is absent or corrupt it returns a fresh instance pre-populated with `defaultHubs`.

## Cross-references
- **Uses:** `Shared/Config/Sources/LocalHubConfig.cs`, `Shared/Config/Sources/RemoteHubConfig.cs`, `Shared/Config/Sources/RemotePluginConfig.cs`, `Shared/Config/Sources/LocalPluginConfig.cs`, `Shared/Config/Sources/ModConfig.cs`, `Shared/LogFile.cs`
- **Used by:** [ConfigManager.cs](ConfigManager.cs.md), [PluginList.cs](../PluginList.cs.md)
