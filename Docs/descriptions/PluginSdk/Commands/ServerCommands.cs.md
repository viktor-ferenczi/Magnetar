# PluginSdk/Commands/ServerCommands.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** static class · **Lines:** 49

## Summary
`ServerCommands` is the plugin-facing static facade for command registration, analogous to how `Harmony.PatchAll(Assembly)` is the entry point for Harmony patches. A plugin calls `ServerCommands.Register(Assembly.GetExecutingAssembly())` (or the explicit-types overload) to hand its command modules to the host. The host installs its `ICommandRegistrar` implementation once at startup via the `Registrar` property; if a plugin calls `Register` when no registrar is installed (e.g., on a host that does not support commands), a clear `InvalidOperationException` is thrown rather than a null-reference.

## Types

### `ServerCommands` — static class, public

Thin pass-through wrapper. Ownership of registered commands is attributed to the assembly argument, not to a plugin object, so registration can happen at any lifecycle point (not only during `Init()`).

- **Properties:**
  - `Registrar` — `ICommandRegistrar` set by the host at startup. `set` is public so the host can assign it; plugins must not assign it.
- **Methods:**
  - `Register(Assembly assembly)` — Delegates to `Require().Register(assembly)`. The idiomatic plugin call: `ServerCommands.Register(Assembly.GetExecutingAssembly())`.
  - `Register(Assembly assembly, params Type[] moduleTypes)` — Delegates to `Require().Register(assembly, moduleTypes)`. Use when you want the command class set to be compiler-checked rather than discovered by reflection scan.
  - `Require() → ICommandRegistrar` (private static) — Returns `Registrar` or throws `InvalidOperationException` with a descriptive message if it is `null`.

## Cross-references
- **Uses:** `PluginSdk/Commands/ICommandRegistrar.cs`
- **Used by:** [PluginLoader.cs](../../Legacy/Loader/PluginLoader.cs.md)
