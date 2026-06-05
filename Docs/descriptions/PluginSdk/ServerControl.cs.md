# PluginSdk/ServerControl.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk` · **Kind:** static class + enum · **Lines:** 142

## Summary

Exposes the dedicated server's lifecycle controls (save, reload config, quit, restart) as a stable plugin-facing API, decoupled from the host launcher implementation. The host binds real delegates at startup via the internal `Bind` method; before binding (e.g. in unit tests) all calls are safe no-ops. The file also declares `ServerTerminationKind`, the discriminated union carried by the `Terminating` event. The `Terminating` event fires when an admin explicitly drives the server lifecycle from in-game (the `!quit`/`!stop`/`!restart` commands) and is distinct from OS-level SIGTERM/SIGINT, which are handled directly by the host without raising this event. Access to the internal `Bind` and `RaiseTerminating` members is restricted to `MagnetarInterim`, `MagnetarLegacy`, and `PluginSdkTests` via `InternalsVisibleTo`.

## Types

### `ServerTerminationKind` — enum, public

Discriminates the two shutdown intents communicated through `ServerControl.Terminating`.

- `Shutdown` — an admin requested a permanent stop (e.g. `!quit` / `!stop`)
- `Restart` — the server will come back up after the current process exits (e.g. `!restart`)

---

### `ServerControl` — static class, public

Plugin-facing facade for server lifecycle operations. Internally holds six private `Func<bool>` / `Action` delegate fields, each initialized to a safe no-op lambda. `Bind` replaces all six atomically (null-safe). All public methods delegate to the corresponding field with no additional locking; thread safety is documented as the host's responsibility (marshalling to the game update thread, null-guarding sessions).

- **Fields (private static):**
  - `saveWorld` (`Func<bool>`) — invoked by `SaveWorld()`; no-op returns `false` until bound
  - `reloadConfig` (`Func<bool>`) — invoked by `ReloadConfig()`; no-op returns `false` until bound
  - `saveAndQuit` (`Action`) — invoked by `SaveAndQuit()`
  - `saveAndRestart` (`Action`) — invoked by `SaveAndRestart()`
  - `quitWithoutSaving` (`Action`) — invoked by `QuitWithoutSaving()`
  - `restartWithoutSaving` (`Action`) — invoked by `RestartWithoutSaving()`

- **Events:**
  - `Terminating` (`event Action<ServerTerminationKind>`, public static) — raised once per admin-initiated shutdown or restart, before any save or plugin disposal, on the thread performing the termination; plugins may subscribe to flush short-lived network messages or persisted state; handlers must return promptly — an exception from one handler is logged to `MyLog.Default` and does not block the remaining handlers or teardown

- **Methods (public):**
  - `SaveWorld() → bool` — saves the current session without stopping the server; returns `false` when no session is loaded or the host is unbound
  - `ReloadConfig() → bool` — saves the session then re-reads the dedicated server config and applies runtime-safe settings (e.g. MOTD); returns `false` when unbound or no session
  - `SaveAndQuit()` — saves the session and exits the process with code 0
  - `SaveAndRestart()` — saves the session and replaces the process using the original argv/env/cwd captured at startup
  - `QuitWithoutSaving()` — immediate exit, no save
  - `RestartWithoutSaving()` — immediate process-replace, no save

- **Methods (internal):**
  - `RaiseTerminating(ServerTerminationKind kind)` — host-only; iterates the invocation list of `Terminating` and calls each subscriber individually inside a try/catch, logging failures to `MyLog.Default.Error` so a faulting plugin cannot block teardown
  - `Bind(Func<bool>, Func<bool>, Action, Action, Action, Action)` — host-only; installs the six real delegate implementations; null arguments fall back to the no-op defaults; called once at launcher startup

## Cross-references

- **Uses:** `VRage.Utils.MyLog` (SE DS assembly, for error logging in `RaiseTerminating`)
- **Used by:** _none within the repository_
