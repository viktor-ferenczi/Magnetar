# PluginSdkTests/LoggingTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test class · **Lines:** 198

## Summary
Specifies the PluginSdk logging subsystem: `Logger`, `LogEntry`, `ILogSink`, `LogLevel`, `QuasarLogSink`, `MagnetarLogSink`, and `LogEnvironment`. Tests confirm that `Logger` stamps every entry with the plugin name, level, managed thread id, and UTC timestamp; that exception and arbitrary data payloads are captured independently and together; that null constructor arguments throw `ArgumentNullException`; that `QuasarLogSink.Format` emits ISO 8601 microsecond-precision UTC JSON with correct field names and conditional `data`/`exception` properties; that `MagnetarLogSink.Format` produces a human-readable line with a JSON suffix for the data payload; and that `LogEnvironment` selects the right sink implementation by inspecting an environment variable, enabling host-level switching between Quasar-managed JSON output and the native Magnetar text output without recompilation.

## Types

### `LoggingTests` — class, public

Xunit test class. Contains one private nested stub type and all test methods.

- **Nested types:**
  - `CapturingSink` (private sealed class : `ILogSink`) — accumulates `LogEntry` structs in a `List<LogEntry>` by implementing `Write(in LogEntry)`. Used by all tests that want to inspect what was sent to the sink.

- **Methods (test methods):**
  - `Logger_StampsPluginNameLevelThreadAndUtcTime` — constructs `Logger("MyPlugin", sink)`, calls `log.Warning("hello")`, asserts `PluginName`, `Level`, `Message`, `ThreadId` (matches `Environment.CurrentManagedThreadId`), `UtcTimestamp.Kind` (UTC), and that `Exception` is null.
  - `Logger_Error_CapturesException` — `log.Error("failed", ex)` stores the exception in `entry.Exception` and leaves `entry.Data` null.
  - `Logger_CapturesDataPayload` — `log.Info("downloaded", payload)` stores the anonymous-object payload in `entry.Data` and leaves `entry.Exception` null.
  - `Logger_Error_CapturesExceptionAndData` — `log.Error("failed", ex, payload)` fills both `entry.Exception` and `entry.Data`.
  - `Logger_NullArguments_Throw` — null plugin name or null sink throws `ArgumentNullException`.
  - `QuasarLogSink_FormatsIso8601MicrosecondUtcJson` — calls `QuasarLogSink.Format(in entry)` with a known `DateTime` (tick-precise), parses the result as JSON, and asserts the `timestamp` field is `"2026-05-30T12:34:56.123456Z"` (6 decimal digits, `Z` suffix); also checks `level`, `plugin`, `thread`, `message` fields and that `data`/`exception` are absent.
  - `QuasarLogSink_NestsDataPayloadAsJsonObject` — data payload serialised as a nested JSON object under the `"data"` key.
  - `MagnetarLogSink_AppendsDataPayloadAsJsonText` — `MagnetarLogSink.Format` produces a line containing `[MyPlugin] [thread N] message` with a `{...}` JSON suffix.
  - `QuasarLogSink_IncludesExceptionWhenPresent` — exception with a real stack trace serialised as a string under `"exception"` key; asserts `InvalidOperationException` appears in the value.
  - `QuasarLogSink_WritesSingleJsonLineToInjectedWriter` — instantiates `QuasarLogSink` with a `Action<string>` writer, calls `Write`, asserts output is a single non-newline-containing JSON object.
  - `LogEnvironment_SelectsSinkFromEnvironmentVariable` — sets/clears the `LogEnvironment.QuasarEnvironmentVariable` env var, asserts `LogEnvironment.IsManagedByQuasar()` and `LogEnvironment.CreateDefaultSink()` return the correct type in each case, restores the original value in a `finally` block.

## Cross-references
- **Uses:**
  - `PluginSdk/Logging/` — `Logger`, `LogEntry`, `ILogSink`, `LogLevel`, `QuasarLogSink`, `MagnetarLogSink`, `LogEnvironment`
  - `System.Text.Json` — JSON parsing in assertion helpers
- **Used by:** _none within the repository_
