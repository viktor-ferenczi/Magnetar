# Legacy/Patch/Patch_MyScriptManager.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 78

## Summary
Postfix-patches `MyScriptManager.LoadData` to compile and load scripts for client-side `ModPlugin` entries after SE has processed all normal session mods. This is the script-loading counterpart to `Patch_MyDefinitionManager`: definitions are injected by that patch; scripts are injected here. The patch also injects the `"PULSAR"` conditional compilation symbol into the Roslyn compiler so mod scripts can use `#if PULSAR` guards, and coordinates with `Patch_MyDefinitionErrors` to redirect Roslyn diagnostic output through Magnetar's logger.

## Types

### Patch_MyScriptManager — static class, public
Harmony Postfix on `VRage.Scripting.MyScriptManager.LoadData()`, applied in the `"Late"` patch category. The static constructor resolves two private members by reflection and caches them for use in the Postfix:

- `loadScripts`: a delegate wrapping the private instance method `MyScriptManager.LoadScripts(string path, MyModContext mod)`, used to trigger per-mod script compilation without a direct reference to the internal API.
- `conditionalSymbols`: a `FieldInfo` for `MyScriptCompiler.m_conditionalCompilationSymbols` (`HashSet<string>`), used to inject and then remove the `"PULSAR"` compilation symbol.

The `Postfix` method:
1. Builds the set of current session mod workshop IDs.
2. Adds `"PULSAR"` to `MyScriptCompiler.Static`'s conditional-compilation symbol set.
3. Calls `Patch_MyDefinitionErrors.RedirectModLogging(true)` to enable Pulsar-side Roslyn diagnostics.
4. For each `ModPlugin` in the active profile, calls `loadScripts(__instance, mod.ModLocation, mod.GetModContext())` to compile and load the mod's scripts.
5. Calls `RedirectModLogging(false)` to restore normal SE error handling.
6. Removes `"PULSAR"` from the compiler symbol set.

- **Fields:** `loadScripts — private static Action<MyScriptManager, string, MyModContext>; cached delegate for MyScriptManager.LoadScripts (private)` · `conditionalSymbols — private static FieldInfo; FieldInfo for MyScriptCompiler.m_conditionalCompilationSymbols` · `ConditionalSymbol — const string = "PULSAR"`
- **Properties:** `ConditionalSymbols — HashSet<string>; reads the live conditional-compilation symbol set from MyScriptCompiler.Static via the cached FieldInfo`
- **Methods:** `static Patch_MyScriptManager() — resolves and caches loadScripts delegate and conditionalSymbols FieldInfo` · `Postfix(MyScriptManager __instance) — Harmony Postfix; injects PULSAR symbol, compiles client-mod scripts, restores state`

## Cross-references
- **Uses:** `Legacy/Patch/Patch_MyDefinitionErrors.cs` (`RedirectModLogging`), `Legacy/Extensions/ModPlugin.cs` (`GetModContext`), `Shared/Config/ConfigManager.cs`, `Shared/Data/PluginList.cs`, `Shared/LogFile.cs`, `VRage.Scripting.MyScriptManager.LoadData` (patched target), `VRage.Scripting.MyScriptCompiler.m_conditionalCompilationSymbols` (private field), `Sandbox.Game.World.MySession.Static.Mods`
- **Used by:** _none within the repository_
