# Shared/Stats/Model/PluginStats.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Stats.Model` · **Kind:** class · **Lines:** 21

## Summary
Top-level response container returned by the `/Stats` REST endpoint. It wraps the per-plugin statistics map and carries a server-issued voting token that the client must present on subsequent vote requests, acting as a lightweight anti-spoofing mechanism. `ConfigManager` caches the deserialized instance so that the UI layer can read it without additional network calls.

## Types
### `PluginStats` — class, public
Aggregates all per-plugin statistics into a keyed dictionary and provides a convenience accessor. Deserialized by `StatsClient.DownloadStats` from the JSON response body and stored in `ConfigManager.Instance.Stats`.

- **Properties:**
  - `Stats` — `Dictionary<string, PluginStat>` mapping plugin ID strings to their individual `PluginStat` records; initialized to an empty dictionary so callers never encounter `null`.
  - `VotingToken` — opaque server-generated token required by `StatsClient.Vote`; only populated when the request included a `playerHash` query parameter (i.e. when `DataHandlingConsent` is enabled); cached locally in `StatsClient.votingToken`.

- **Methods:**
  - `GetStatsForPlugin(PluginData data) → PluginStat` — looks up statistics by `data.Id`; returns a default-constructed (all-zero) `PluginStat` if the plugin is absent from the dictionary, ensuring callers always get a safe value without null-checks.

## Cross-references
- **Uses:** `Shared/Data/PluginData.cs` (parameter type of `GetStatsForPlugin`)
- **Used by:** [StatsClient.cs](../StatsClient.cs.md), [ConfigManager.cs](../../Config/ConfigManager.cs.md)
