# PluginSdk/Logging/QuasarLogSink.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Logging` · **Kind:** sealed class · **Lines:** 89

## Summary
The `ILogSink` used when the server process is managed by the Quasar Agent. Each log entry is serialized as a compact single-line JSON object. `Write` then does two things with that line: it writes it through the `Action<string>` delegate (the default constructor uses `Console.Out`, landing the line in the managed server's on-disk log file), and it raises `LogEnvironment.RaiseLineEmitted` so the in-process Quasar Agent can ship the line to Quasar over its network channel (resilient to Quasar restarts and reconnects to a detached server daemon, unlike stdout capture). A secondary constructor accepting a custom `writeLine` delegate is provided for tests and custom redirection. Timestamps carry microsecond precision in ISO 8601 UTC format.

## Types

### `QuasarLogSink` — sealed class, public : `ILogSink`
Renders entries as structured JSON for machine consumption by the Quasar Agent.

- **Fields:**
  - `TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'"` (private const) — ISO 8601 UTC format with six fractional-second digits (microsecond precision); `T` and `Z` are quoted literals
  - `writeLine : Action<string>` (private readonly) — receives each finished JSON line; null guard on construction
- **Methods:**
  - `QuasarLogSink()` (public, parameterless) — constructs with `line => Console.Out.WriteLine(line)` as the writer; the standard deployment constructor
  - `QuasarLogSink(Action<string> writeLine)` (public) — primary constructor; accepts any line-writing delegate
  - `Write(in LogEntry entry)` — formats the entry once, then both calls `writeLine(line)` (stdout/on-disk log) and `LogEnvironment.RaiseLineEmitted(line)` (in-process relay to the Quasar Agent)
  - `Format(in LogEntry entry) : string` (public static) — builds a `LogRecord`, populates all fields from the entry (data via `LogJson.ToElement`, exception via `Exception.ToString()`), and serializes with `LogJson.Options`; exposed publicly for tests
- **Nested types:**
  - `LogRecord` (private sealed class) — POCO with `[JsonPropertyName]` attributes controlling JSON key names and property order: `timestamp`, `level`, `plugin`, `thread`, `message`, `data` (`JsonElement?`, omitted when null), `exception` (string, omitted when null)

## Cross-references
- **Uses:** [LogEnvironment.cs](LogEnvironment.cs.md) (`RaiseLineEmitted`), `PluginSdk/Logging/ILogSink.cs`, `PluginSdk/Logging/LogEntry.cs`, `PluginSdk/Logging/LogJson.cs`, `System.Text.Json` (BCL), `System.Globalization` (BCL)
- **Used by:** [LogEnvironment.cs](LogEnvironment.cs.md), [LoggingTests.cs](../../PluginSdkTests/LoggingTests.cs.md)
