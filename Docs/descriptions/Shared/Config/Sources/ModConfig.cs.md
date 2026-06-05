# Shared/Config/Sources/ModConfig.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Config` · **Kind:** class · **Lines:** 8

## Summary
`ModConfig` is the configuration record for a Steam Workshop mod source. It is serialised as a `<Mod>` element inside `SourcesConfig` and identifies a mod by its Steam Workshop numeric ID, allowing Magnetar to manage Workshop mods alongside plugin sources under a single configuration file.

## Types

### ModConfig — class, public

Plain data-transfer object for one Steam Workshop mod entry.

- **Properties:** `Name` — display name for the mod; `ID` — Steam Workshop item ID (`long`); `Enabled` — whether this mod is active.

## Cross-references
- **Uses:** _(none — pure DTO)_
- **Used by:** [SourcesConfig.cs](../SourcesConfig.cs.md), [PluginList.cs](../../PluginList.cs.md)
