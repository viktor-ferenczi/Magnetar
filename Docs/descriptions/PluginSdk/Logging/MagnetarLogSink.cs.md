# PluginSdk/Logging/MagnetarLogSink.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Logging` · **Kind:** sealed class · **Lines:** 58

## Summary
The `ILogSink` used when the server runs under standalone Magnetar (no Quasar Agent). It forwards every entry to the Space Engineers Dedicated Server game log through `VRage.Utils.MyLog.Default`, which is the standard SE logging target shared by the game, Torch, and Magnetar itself. The sink safely no-ops when `MyLog.Default` is null (i.e., before the game log is initialized), making it safe to use from plugin static constructors. Data payloads are appended as compact JSON text on the same line; exceptions are appended on a following line.

## Types

### `MagnetarLogSink` — sealed class, public : `ILogSink`
Bridges the SDK logging system to the SE DS `MyLog` API.

- **Methods:**
  - `Write(in LogEntry entry)` — obtains `MyLog.Default`; returns immediately when null; calls `log.Log(severity, "{0}", line)` passing the pre-formatted line as the format argument (not the format string) to prevent `{`/`}` characters in messages or JSON from being misinterpreted by `MyLog`'s internal `string.Format`
  - `Format(in LogEntry entry) : string` (public static) — builds the body string: `[plugin] [thread N] message`, appending `" " + LogJson.Serialize(data)` when `entry.Data` is non-null, and `Environment.NewLine + exception.ToString()` when `entry.Exception` is non-null; exposed publicly to aid testing and reuse by other sinks
  - `ToSeverity(LogLevel level) : MyLogSeverity` (private static) — switch expression mapping each `LogLevel` value to the identical `MyLogSeverity` member; defaults to `MyLogSeverity.Info` for any unknown future level

## Cross-references
- **Uses:** `PluginSdk/Logging/ILogSink.cs`, `PluginSdk/Logging/LogEntry.cs`, `PluginSdk/Logging/LogLevel.cs`, `PluginSdk/Logging/LogJson.cs`, SE DS assembly `VRage.Library` (`VRage.Utils.MyLog`, `VRage.Utils.MyLogSeverity`)
- **Used by:** [LogEnvironment.cs](LogEnvironment.cs.md), [LoggingTests.cs](../../PluginSdkTests/LoggingTests.cs.md)
