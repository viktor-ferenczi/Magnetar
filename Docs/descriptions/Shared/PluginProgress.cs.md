# Shared/PluginProgress.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared` · **Kind:** static class · **Lines:** 45

## Summary
Plain-text console progress reporter for plugin download and compilation, replacing the former WinForms splash screen that does not exist on the headless DS. Each report writes a `[Magnetar] ...` line to stdout and mirrors it to the log file, and counters track totals for the final summary line.

## Types
### `PluginProgress` — static class, public
Tracks running totals of downloaded and compiled plugins and emits progress lines.
- **Properties:** `Downloaded` — count of download reports so far (`{ get; private set; }`); `Compiled` — count of completed compilations (`{ get; private set; }`).
- **Methods:** `ReportDownloading(string name)` — increments `Downloaded`, writes "Downloading {name}"; `ReportCompiling(string name)` — writes "Compiling {name}" (no counter); `ReportCompiled(string name)` — increments `Compiled`, writes "Compiled {name}"; `ReportSummary(int loaded, int implicitlyLoaded)` — writes download/compile totals and "Loaded N plugins (M implicit)"; `Plural(int)` — "1 plugin" / "N plugins" helper; `Write(string)` — writes a `[Magnetar] ` prefixed line to `Console.Out` and to `LogFile`.

## Cross-references
- **Uses:** `Shared/LogFile.cs`; `System.Console`.
- **Used by:** [GitHubPlugin.cs](Data/GitHubPlugin.cs.md), [LocalFolderPlugin.cs](Data/LocalFolderPlugin.cs.md), [Loader.cs](Loader.cs.md)
