# Legacy/Patch/Patch_MySessionLoader.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** class, internal · **Lines:** 34

## Summary
Contains two Harmony Prefix patches on `MySessionLoader.LoadMultiplayerScenarioWorld` and `MySessionLoader.LoadMultiplayerSession` that enforce a "trusted mods" security policy. When the `-hardened` command-line flag is active (`Flags.TrustedMods`), any mod in the world checkpoint that is not locally installed via Steam is stripped from the mod list before the session loads, preventing untrusted or unknown workshop items from running on the server.

## Types

### Patch_MySessionLoader — class, internal
Applied in the `"Early"` patch category. Hosts two independent Harmony Prefix patches on the two multiplayer-session load entry points. Both patches share identical logic: when `Flags.TrustedMods` is `true`, call `world.Checkpoint.Mods.RemoveAll(SteamMods.IsModUntrusted)` to remove any `ModItem` whose `PublishedServiceName` is not `"Steam"` or whose item state does not include `k_EItemStateInstalled`. The patches return `void` (no return value), so the original methods continue executing with the now-filtered mod list.

- **Methods:** `Patch_LoadMultiplayerScenarioWorld(MyObjectBuilder_World world, MyMultiplayerBase multiplayerSession) — Harmony Prefix on MySessionLoader.LoadMultiplayerScenarioWorld; strips untrusted mods when hardened mode is active` · `Patch_LoadMultiplayerSession(MyObjectBuilder_World world, MyMultiplayerBase multiplayerSession) — Harmony Prefix on MySessionLoader.LoadMultiplayerSession; strips untrusted mods when hardened mode is active`

## Cross-references
- **Uses:** `Legacy/Loader/SteamMods.cs` (`SteamMods.IsModUntrusted`), `Shared/Flags.cs` (`Flags.TrustedMods`), `Sandbox.Game.World.MySessionLoader.LoadMultiplayerScenarioWorld` and `LoadMultiplayerSession` (patched targets), `Sandbox.Engine.Multiplayer.MyMultiplayerBase`, `VRage.Game.MyObjectBuilder_World`
- **Used by:** _none within the repository_
