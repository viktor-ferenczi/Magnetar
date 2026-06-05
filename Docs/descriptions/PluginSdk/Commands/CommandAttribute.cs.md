# PluginSdk/Commands/CommandAttribute.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** sealed class (Attribute) · **Lines:** 54

## Summary
`CommandAttribute` is the marker that turns a public instance method of a `CommandModule` subclass into a chat command handler. The `Command` string is a space-separated sub-path appended to the module's root prefix to form the full chat syntax (e.g., `[Command("grid list")]` on a module rooted at `"ess"` is invoked as `!ess grid list`). An empty `Command` string designates the root-level default command, executed when the player types just `!prefix` with no further words.

## Types

### `CommandAttribute` — sealed class : `Attribute`, public

Applied with `[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]`. The attribute is discoverable by `CommandRegistry.RegisterModule`, which reads it via `GetCustomAttribute<CommandAttribute>(false)` on each public declared instance method of the module type.

- **Properties:**
  - `Command` — Space-separated path relative to the module root (e.g., `"grid list"`). Matched case-insensitively at dispatch time.
  - `Description` — One-line description shown in `!prefix` overview listings and global `!help`.
  - `HelpText` — Extended help shown for `!prefix help <command>`; falls back to `Description` when `null`.

## Cross-references
- **Uses:** `PluginSdk/Commands/CommandModule.cs` (applied to methods of its subclasses), `PluginSdk/Commands/CommandRootAttribute.cs` (conceptually paired)
- **Used by:** [CommandRegistry.cs](CommandRegistry.cs.md)
