# PluginSdk/Commands/ICommandRegistrar.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** interface · **Lines:** 26

## Summary
`ICommandRegistrar` is the host-implemented sink through which plugins register their command modules. It is surfaced to plugins indirectly through the `ServerCommands` static facade; plugins never hold a reference to it directly. The interface exists so the host implementation can be swapped in tests or across host variants (Legacy/Interim) without changing the plugin-facing API.

## Types

### `ICommandRegistrar` — interface, public

Defines two overloads of `Register`:

- **Methods:**
  - `Register(Assembly assembly)` — Scans the assembly for all non-abstract `CommandModule` subclasses carrying `[CommandRoot]` and registers them, attributing ownership to that assembly.
  - `Register(Assembly assembly, params Type[] moduleTypes)` — Registers only the explicitly listed `CommandModule` types, owned by the given assembly. Preferred when the command class set should be compiler-verified rather than discovered at runtime.

## Cross-references
- **Uses:** `PluginSdk/Commands/CommandModule.cs`, `PluginSdk/Commands/CommandRootAttribute.cs`
- **Used by:** [CommandService.cs](../../Legacy/Commands/CommandService.cs.md), [ServerCommands.cs](ServerCommands.cs.md)
