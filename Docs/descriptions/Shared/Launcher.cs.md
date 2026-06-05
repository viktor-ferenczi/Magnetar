# Shared/Launcher.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared` · **Kind:** class · **Lines:** 52

## Summary
Performs pre-launch sanity checks before Magnetar starts the SE Dedicated Server: refuses to start if the SE process is already running, rejects the removed `-plugin` switch, and verifies that an app `.config` exists when the SE folder ships one. Constructed with the path to the SE executable (primary constructor parameter `sePath`).

## Types
### `Launcher(string sePath)` — class, public
Wraps the SE executable path and exposes guard methods the launcher entry point calls before bootstrapping plugins.
- **Methods:** `CanStart()` — returns false (after showing a message) if SE is already running, or if `-plugin` is present on the command line (support dropped; user is told to use `-sources`); otherwise true. `IsSpaceEngineersRunning()` — enumerates processes whose name matches the SE executable's file name and checks whether any process's `MainModule.FileName` equals `sePath` (case-insensitive), so only *this* SE install counts. `VerifyConfig()` — returns false when the SE folder contains a `*.config` file but the entry assembly's expected `<entryAssembly>.config` is missing (indicating a broken/incomplete install); otherwise true.

## Cross-references
- **Uses:** `Shared/Tools.cs` (`ShowMessage`, `GetFiles`); `System.Diagnostics.Process`, `System.Reflection`.
- **Used by:** _none within the repository_
