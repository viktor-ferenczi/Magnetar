# Legacy/Patch/Patch_ComponentRegistered.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 20

## Summary
Prefix-patches `MySession.RegisterComponentsFromAssembly` to inject plugin-provided session components at exactly the right moment in the SE session lifecycle. SE calls `RegisterComponentsFromAssembly` once per assembly; this patch detects the call for the main game assembly and uses it as a synchronization point to tell `PluginLoader` to register session components from all loaded plugins.

## Types

### Patch_ComponentRegistered — static class, public
Harmony Prefix on `Sandbox.Game.World.MySession.RegisterComponentsFromAssembly(Assembly, bool, MyModContext)`, applied in the `"Early"` patch category. When the assembly being registered is `MyPlugins.GameAssembly` (the primary SE game assembly), calls `PluginLoader.Instance?.RegisterSessionComponents()`, which iterates over every active `PluginInstance` and lets each one register its session components with `MySession.Static`. This ensures plugin session components are registered in the same pass as the core game components, preserving the expected initialization order.

- **Methods:** `Prefix(Assembly assembly) — Harmony Prefix; triggers plugin session-component registration when the game assembly is the assembly being registered`

## Cross-references
- **Uses:** `Legacy/Loader/PluginLoader.cs` (`PluginLoader.Instance.RegisterSessionComponents`), `VRage.Plugins.MyPlugins.GameAssembly`, `Sandbox.Game.World.MySession.RegisterComponentsFromAssembly` (patched target)
- **Used by:** _none within the repository_
