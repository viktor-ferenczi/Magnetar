# PluginSdk/Logging/QuasarLogSink.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Logging` · **Kind:** sealed class · **Lines:** 76

## Summary
The `ILogSink` used when the server process is managed by the Quasar Agent. Each log entry is serialized as a compact single-line JSON object and written through an `Action<string>` delegate. The default constructor (used by `LogEnvironment.CreateDefaultSink`) writes to `Console.Out`, which the Quasar Agent captures from the managed process's stdout. A secondary constructor accepting a custom `writeLine` delegate is provided for tests and for future wiring of a dedicated inter-process transport. Timestamps carry microsecond precision in ISO 8601 UTC format.

## Types

### `QuasarLogSink` — sealed class, public : `ILogSink`
Renders entries as structured JSON for machine consumption by the Quasar Agent.

- **Fields:**
  - `TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'"` (private const) — ISO 8601 UTC format with six fractional-second digits (microsecond precision); `T` and `Z` are quoted literals
  - `writeLine : Action<string>` (private readonly) — receives each finished JSON line; null guard on construction
- **Methods:**
  - `QuasarLogSink()` (public, parameterless) — constructs with `line => Console.Out.WriteLine(line)` as the writer; the standard deployment constructor
  - `QuasarLogSink(Action<string> writeLine)` (public) — primary constructor; accepts any line-writing delegate
  - `Write(in LogEntry entry)` — calls `writeLine(Format(in entry))`
  - `Format(in LogEntry entry) : string` (public static) — builds a `LogRecord`, populates all fields from the entry (data via `LogJson.ToElement`, exception via `Exception.ToString()`), and serializes with `LogJson.Options`; exposed publicly for tests
- **Nested types:**
  - `LogRecord` (private sealed class) — POCO with `[JsonPropertyName]` attributes controlling JSON key names and property order: `timestamp`, `level`, `plugin`, `thread`, `message`, `data` (`JsonElement?`, omitted when null), `exception` (string, omitted when null)

## Cross-references
- **Uses:** `PluginSdk/Logging/ILogSink.cs`, `PluginSdk/Logging/LogEntry.cs`, `PluginSdk/Logging/LogJson.cs`, `System.Text.Json` (BCL), `System.Globalization` (BCL)
- **Used by:** [LogEnvironment.cs](LogEnvironment.cs.md), [LoggingTests.cs](../../PluginSdkTests/LoggingTests.cs.md)
