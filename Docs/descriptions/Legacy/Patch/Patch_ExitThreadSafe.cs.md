# Legacy/Patch/Patch_ExitThreadSafe.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** class, public · **Lines:** 20

## Summary
Prefix-patches `MySandboxGame.ExitThreadSafe` to redirect in-game and admin-triggered exit requests through Magnetar's graceful shutdown path. SE's built-in `ExitThreadSafe` hangs in the in-process hosting setup used by Magnetar and does not guarantee a world save; this patch replaces it with `ServerControl.SaveAndQuit()`, the same path taken on SIGTERM.

## Types

### Patch_ExitThreadSafe — class, public
Harmony Prefix on `Sandbox.MySandboxGame.ExitThreadSafe`, applied in the `"Early"` patch category. The prefix calls `ServerControl.SaveAndQuit()` and returns `false` to suppress the original method entirely. `SaveAndQuit` dispatches a save through the SE update-thread fast path and then initiates an orderly process exit, which avoids the hang that the native `ExitThreadSafe` produces under in-process hosting.

- **Methods:** `Prefix() — Harmony Prefix; returns false (skips original); delegates to ServerControl.SaveAndQuit()`

## Cross-references
- **Uses:** `Legacy/Launcher/ServerControl.cs` (`ServerControl.SaveAndQuit`), `Sandbox.MySandboxGame.ExitThreadSafe` (patched target)
- **Used by:** _none within the repository_
