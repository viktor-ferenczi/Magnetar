# Shared/Updater.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared` · **Kind:** class · **Lines:** 209

## Summary
Handles Magnetar's self-update against a GitHub release repo. It compares the running entry-assembly version to the latest GitHub release (stable or pre-release per `Flags.UpdateType`), and when newer, downloads release assets, ensures a compatible external `Updater.exe`, then launches that updater (forwarding the original command line) and exits so the updater can replace the in-use binaries. Constructed with the GitHub `repoName`. Also provides user-facing prompts for SE version changes and broken installs.

## Types
### `Updater(string repoName)` — class, public
Drives the version check and update launch flow; primary-constructor `repoName` is the release repo.
- **Fields (const):** `UpdaterName="Updater"`, `PulsarName="Pulsar"`, `DebugArg="-debug"`.
- **Fields:** `remotePulsarVer` — the discovered remote release version.
- **Methods:** `TryUpdate()` — if updates enabled and a newer release exists, logs and calls `Update()`; `GameUpdatePrompt(Version old, Version new, int fieldCount)` (static) — shows the "Space Engineers has been up/downgraded, rebuild plugins" notice and clears the GitHub cache (`GitHubPlugin.ClearGitHubCache`); `ShowBitrotPrompt()` — reports a broken Pulsar install and exits(1); `ShowUpdateError()` (static) — generic update-failure message; `Update()` — fetches the release JSON, extracts updater and Pulsar asset URLs, downloads a newer `Updater.exe` if needed, clears the GitHub cache, and starts the updater; `TryGetUpdaterInfo` / `TryGetPulsarPath` (static) — parse the release `assets` array for the updater (version parsed from the asset name) and Pulsar download URLs; `GetLocalUpdaterVersion` (static) — reads the local `Updater.exe` assembly version (null if absent); `DownloadUpdater` (static) — streams the remote updater to disk via `GitHub.GetStream`; `StartUpdater` (static) — launches `Updater.exe` with `-caller/-remote/-local` plus the forwarded original args (re-adding `-debug` only if a debugger is attached), then `Environment.Exit(0)`.

## Cross-references
- **Uses:** `Shared/Flags.cs`, `Shared/LogFile.cs`, `Shared/Tools.cs`; Shared.Config (`ConfigManager.PulsarDir`), Shared.Data (`GitHubPlugin.ClearGitHubCache`), Shared.Network (`GitHub` release/JSON/stream APIs); Newtonsoft.Json.Linq; external systems GitHub and the standalone Updater.exe.
- **Used by:** [Program.cs](../Legacy/Program.cs.md)
