# Legacy/Patch/Patch_LoadScripts.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 17

## Summary
Postfix-patches `MyScriptManager.LoadScripts` to trigger plugin entity-component registration at the correct point in session startup. SE calls `LoadScripts` for every mod and also for the base game; this patch catches the base-game invocation (identified by the current session path and `MyModContext.BaseGame`) and calls `PluginLoader.Instance?.RegisterEntityComponents()`, ensuring plugins can register entity components after all scripts are loaded.

## Types

### Patch_LoadScripts — static class, public
Harmony Postfix on `Sandbox.Game.World.MyScriptManager.LoadScripts(string path, MyModContext mod)`, applied in the `"Late"` patch category. The guard `path == MySession.Static.CurrentPath && mod == MyModContext.BaseGame` ensures the hook fires exactly once per session load, on the base-game script-loading pass rather than any mod pass. On match, it calls `PluginLoader.Instance?.RegisterEntityComponents()`, which forwards to each active `PluginInstance`.

- **Methods:** `Postfix(string path, MyModContext mod) — Harmony Postfix; calls RegisterEntityComponents when the base-game script load for the current session is detected`

## Cross-references
- **Uses:** `Legacy/Loader/PluginLoader.cs` (`PluginLoader.Instance.RegisterEntityComponents`), `Sandbox.Game.World.MyScriptManager.LoadScripts` (patched target), `Sandbox.Game.World.MySession.Static.CurrentPath`, `VRage.Game.MyModContext.BaseGame`
- **Used by:** _none within the repository_
