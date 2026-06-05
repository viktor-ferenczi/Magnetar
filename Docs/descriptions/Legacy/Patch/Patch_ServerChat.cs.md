# Legacy/Patch/Patch_ServerChat.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class, public · **Lines:** 50

## Summary
Prefix-patches `MyMultiplayerBase.OnChatMessageReceived_Server` to intercept global chat messages whose text begins with `'!'` and route them through Magnetar's `CommandService` before SE can broadcast them to other players. If a command is successfully handled, the original method is suppressed so the command text is never echoed to the chat log or relayed to clients.

## Types

### Patch_ServerChat — static class, public
Harmony Prefix on `Sandbox.Engine.Multiplayer.MyMultiplayerBase.OnChatMessageReceived_Server(ChatMsg)`, applied in the `"Late"` patch category. The prefix:

1. Returns `true` immediately (lets SE handle normally) if the message channel is not `ChatChannel.Global`.
2. Returns `true` if the text is null/empty or does not start with `'!'`.
3. Returns `true` if `PluginLoader.Instance?.Commands` is null (no command service active).
4. Returns `true` if the sender Steam ID is `0` (server-internal or unauthenticated message).
5. Calls `CommandService.HandleChat(sender, text)`. If the command was handled, returns `false` to suppress the original broadcast; otherwise returns `true`.

All exceptions are caught, logged to `LogFile.Error`, and the original method is allowed to proceed (returns `true`) to avoid killing the multiplayer subsystem on a handler bug.

- **Methods:** `Prefix(ChatMsg msg) — Harmony Prefix; routes '!'-prefixed global chat through CommandService; returns false when a command is handled`

## Cross-references
- **Uses:** `Legacy/Loader/PluginLoader.cs` (`PluginLoader.Instance.Commands`), `Legacy/Commands/CommandService.cs` (`CommandService.HandleChat`), `Shared/LogFile.cs`, `Sandbox.Engine.Multiplayer.MyMultiplayerBase.OnChatMessageReceived_Server` (patched target), `VRage.Network.MyEventContext.Current.Sender`, `Sandbox.Game.Gui.ChatChannel`
- **Used by:** _none within the repository_
