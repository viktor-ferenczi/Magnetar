# Legacy/Commands/ServerCommandResponder.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Commands` · **Kind:** class · **Lines:** 37

## Summary
`ServerCommandResponder` is the `ICommandResponder` implementation that delivers command replies into the SE DS chat system. It bridges the platform-agnostic `CommandReply` value type (from PluginSdk) onto the SE scripted-message API (`MyVisualScriptLogicProvider`). A single shared instance is used for all dispatches; it is passed directly into `CommandDispatcher.Handle` by `CommandService`.

## Types

### `ServerCommandResponder` — class, public : `ICommandResponder`
Translates a `CommandReply` into one of two `MyVisualScriptLogicProvider` calls depending on whether an explicit ARGB `Color` is present. Non-broadcast replies target a specific in-game identity by numeric ID; broadcasts use identity ID `0` (server-wide). The author defaults to `"Server"` when the reply carries no author string, and the font defaults to `MyFontEnum.White` when none is set.

- **Fields:**
  - `Instance` — `public static readonly ServerCommandResponder`; singleton used by `CommandService`

- **Methods:**
  - `Send(in CommandReply reply, in CommandCaller caller)` — implements `ICommandResponder.Send`; early-outs when `reply.HasContent` is false; resolves `target` (0 for broadcast, `caller.IdentityId` otherwise); falls back author/font to defaults; calls `SendChatMessageColored` when `reply.Color.HasValue`, otherwise `SendChatMessage`

## Cross-references
- **Uses:**
  - `PluginSdk/Commands/ICommandResponder.cs` — interface this class implements
  - `PluginSdk/Commands/CommandReply.cs` — `in` parameter with text, author, font, color, broadcast flag
  - `PluginSdk/Commands/CommandCaller.cs` — supplies `IdentityId` for non-broadcast targeting
  - SE DS `Sandbox.Game.MyVisualScriptLogicProvider` — `SendChatMessage` / `SendChatMessageColored` API
  - SE DS `VRage.Game.MyFontEnum` — default font constant (`White`)
  - SE DS `VRageMath.Color` — ARGB color value used in the colored overload
- **Used by:** [CommandService.cs](CommandService.cs.md)
