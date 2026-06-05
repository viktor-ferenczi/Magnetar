# Legacy/Commands/MagnetarCommands.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Commands` · **Kind:** class (multiple) · **Lines:** 46

## Summary
Declares three built-in chat-command modules — `!save`, `!restart`, and `!quit` — that Magnetar registers with `CommandService` before any plugin loads. Because they are registered first and last-registration wins, a plugin may override any of them. Each command offloads its lifecycle work to a worker thread via `Task.Run` so the disk write (for save/restart) can block to completion without stalling the game-update thread; the caller receives an acknowledgement reply immediately via `Context.Respond`.

## Types

### `SaveCommand` — class, public : `CommandModule`
Handles `!save` (bare root, no subcommand). Responds with "Saving world…" then calls `ServerControl.SaveWorld()` on a worker thread, which blocks until the async save completes or a 5-minute timeout elapses.

- **Methods:**
  - `Save()` — `[Command("", "Save the world")]`; responds to caller, dispatches `ServerControl.SaveWorld` on a `Task`

### `RestartCommand` — class, public : `CommandModule`
Handles `!restart`. Responds with "Saving world and restarting the server…" then calls `ServerControl.SaveAndRestart()` on a worker thread (save → raise `Terminating(Restart)` → restart process via `execve`/`Process.Start`).

- **Methods:**
  - `Restart()` — `[Command("", "Save and restart the server")]`; responds to caller, dispatches `ServerControl.SaveAndRestart` on a `Task`

### `QuitCommand` — class, public : `CommandModule`
Handles `!quit`. Responds with "Shutting the server down without saving…" then calls `ServerControl.QuitWithoutSaving()` on a worker thread (raises `Terminating(Shutdown)` → dispose plugins → exit).

- **Methods:**
  - `Quit()` — `[Command("", "Shut the server down without saving")]`; responds to caller, dispatches `ServerControl.QuitWithoutSaving` on a `Task`

## Cross-references
- **Uses:**
  - `PluginSdk/Commands/CommandModule.cs` — base class; provides `Context` (including `Context.Respond`)
  - `PluginSdk/Commands/CommandAttribute.cs` — `[Command]` attribute
  - `PluginSdk/Commands/CommandRootAttribute.cs` — `[CommandRoot]` attribute
  - `Legacy/Launcher/ServerControl.cs` — `SaveWorld`, `SaveAndRestart`, `QuitWithoutSaving` implementations
- **Used by:** [PluginLoader.cs](../Loader/PluginLoader.cs.md)
