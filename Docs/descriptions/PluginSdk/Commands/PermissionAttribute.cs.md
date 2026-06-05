# PluginSdk/Commands/PermissionAttribute.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** sealed class (Attribute) · **Lines:** 28

## Summary
`PermissionAttribute` sets the minimum `MyPromoteLevel` a player must hold to invoke the decorated command. When the attribute is absent, `CommandRegistry.DefaultPermission` (`MyPromoteLevel.Admin`) applies, so commands are inaccessible by default and must be explicitly opened to lower levels. Commands the caller cannot run are hidden from the `!prefix` overview and `!prefix help` listings, so this attribute also controls discoverability.

## Types

### `PermissionAttribute` — sealed class : `Attribute`, public

Applied with `[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]`. The `Level` is read by `CommandRegistry.RegisterModule` and stored in `RegisteredCommand.MinPromoteLevel`. The `MyPromoteLevel` enum is from `VRage.Game.ModAPI` in the SE DS assemblies.

- **Properties:**
  - `Level` — `MyPromoteLevel` value (e.g., `MyPromoteLevel.None` for public commands, `MyPromoteLevel.Admin` for admin-only).

## Cross-references
- **Uses:** `VRage.Game.ModAPI.MyPromoteLevel` (SE DS assembly)
- **Used by:** [CommandRegistry.cs](CommandRegistry.cs.md)
