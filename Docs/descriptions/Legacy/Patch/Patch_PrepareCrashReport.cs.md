# Legacy/Patch/Patch_PrepareCrashReport.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class, internal · **Lines:** 44

## Summary
Prefix-patches `VRage.Platform.Windows.MyCrashReporting.PrepareCrashAnalyticsReporting` to redirect the SE crash reporter to run the correct `SpaceEngineers.exe` binary, which in Magnetar's in-process hosting model is not necessarily the process that crashed. The patch constructs and launches the crash-reporter process manually using the stored `SpaceEngineersPath` value, then returns `false` to suppress the original method.

## Types

### Patch_PrepareCrashReport — static class, internal
Harmony Prefix on `VRage.Platform.Windows.MyCrashReporting.PrepareCrashAnalyticsReporting`, matched by assembly-qualified string `"VRage.Platform.Windows.MyCrashReporting, VRage.Platform.Windows"`, applied in the `"Early"` patch category. When SE's crash-handling code calls `PrepareCrashAnalyticsReporting`, the prefix:

1. Selects the report flag: `"-reporX"` for unsupported-GPU crashes, `"-report"` for all others.
2. Quotes all string arguments (`logPath`, `info.GameName`, `info.AppVersion`, `info.AnalyticId`).
3. Starts a new `Process` with `FileName = SpaceEngineersPath` and the assembled argument list, using `UseShellExecute = false`.
4. Returns `false` to prevent the original implementation from running.

`SpaceEngineersPath` is a public static field that must be set by the launcher before any crash can occur; it holds the absolute path to the SE executable that should be invoked for crash reporting.

The inline comment notes this is a placeholder: a future Pulsar/Magnetar crash screen is intended to replace it.

- **Fields:** `SpaceEngineersPath — string; absolute path to SpaceEngineers.exe; set externally by the launcher; used as the FileName for the crash-reporter process`
- **Methods:** `Prefix(string logPath, bool GDPRConsent, CrashInfo info, bool isUnsupportedGpu) — Harmony Prefix; launches the SE crash reporter via Process.Start with the configured executable path; returns false`

## Cross-references
- **Uses:** `VRage.Platform.Windows.MyCrashReporting.PrepareCrashAnalyticsReporting` (patched target; accessed by name string), `VRage.CrashInfo`, `System.Diagnostics.Process`
- **Used by:** [Program.cs](../Program.cs.md)
