# Legacy/Patch/Patch_MyDefinitionManager.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 42

## Summary
Prefix-patches `MyDefinitionManager.LoadData` to inject client-side mod definitions for any `ModPlugin` entries in the active Magnetar configuration profile before SE processes the mod list. This is the definition-loading half of client-mod support: it adds the workshop mod items to the `mods` list that SE will subsequently use to find and load SBC definition files.

## Types

### Patch_MyDefinitionManager — static class, public
Harmony Prefix on `Sandbox.Definitions.MyDefinitionManager.LoadData(ref List<MyObjectBuilder_Checkpoint.ModItem> mods)`, applied in the `"Early"` patch category. The prefix:

1. Builds a `HashSet<ulong>` of workshop IDs already present in the mod list (the world's own mods).
2. Queries `ConfigManager.Instance.List.GetModPlugins(current, currentMods)` to enumerate `ModPlugin` entries that belong to the active profile and are not already in the session mod list.
3. For each `ModPlugin`, calls `mod.GetModItem()` (an extension method in `Legacy/Extensions/ModPlugin.cs`) to produce a `MyObjectBuilder_Checkpoint.ModItem` pointing at the locally installed workshop folder, and appends it to a copy of the list.
4. Replaces the `ref mods` parameter with the augmented list.

Errors are caught, logged, and re-thrown so the original exception still propagates.

- **Methods:** `Prefix(ref List<MyObjectBuilder_Checkpoint.ModItem> mods) — Harmony Prefix; injects ModPlugin mod-items into the definition-loading mod list`

## Cross-references
- **Uses:** `Shared/Config/ConfigManager.cs` (`ConfigManager.Instance`), `Shared/Data/PluginList.cs` (`GetModPlugins`), `Shared/Data/Profile.cs` (`ConfigManager.Instance.Profiles.Current`), `Legacy/Extensions/ModPlugin.cs` (`ModPlugin.GetModItem`), `Shared/LogFile.cs`, `Sandbox.Definitions.MyDefinitionManager.LoadData` (patched target)
- **Used by:** _none within the repository_
