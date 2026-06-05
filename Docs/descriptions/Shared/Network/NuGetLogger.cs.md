# Shared/Network/NuGetLogger.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Network` · **Kind:** class · **Lines:** 87

## Summary
`NuGetLogger` adapts the NuGet SDK's `ILogger` interface to Magnetar's `LogFile` / NLog pipeline. Every NuGet log message is forwarded to `LogFile.WriteLine` with the `[NuGet]` prefix and a mapped NLog log level. This allows NuGet's internal diagnostic output (downloads, dependency resolution, cache hits) to appear alongside the rest of Magnetar's server log without requiring a separate logging configuration.

## Types

### `NuGetLogger` — class, public : `NuGet.Common.ILogger`

Stateless adapter; all interface methods delegate to one of two internal paths:

- `Log(LogLevel, string)` — the canonical sink: calls `LogFile.WriteLine(data, ConvertLogLevel(level))`
- `Log(ILogMessage)` — extracts `Level` and `Message` from the message object and routes to the string overload
- `ConvertLogLevel(NuGet.Common.LogLevel)` — maps NuGet's six levels to NLog levels: `Debug`/`Verbose` → `NLog.LogLevel.Debug`; `Information`/`Minimal` → `NLog.LogLevel.Info`; `Warning` → `NLog.LogLevel.Warn`; `Error` → `NLog.LogLevel.Error`; anything else → `Info`
- Async variants `LogAsync(LogLevel, string)` and `LogAsync(ILogMessage)` call the synchronous counterparts and return `Task.CompletedTask`
- Level-specific helpers (`LogDebug`, `LogError`, `LogInformation`, `LogInformationSummary`, `LogMinimal`, `LogVerbose`, `LogWarning`) each call `Log(level, data)` with the appropriate `LogLevel` enum value

## Cross-references
- **Uses:**
  - `Shared/LogFile.cs` — `LogFile.WriteLine(string, NLog.LogLevel)`
  - External: `NuGet.Common.ILogger`, `NuGet.Common.LogLevel`, `NuGet.Common.ILogMessage`
  - External: `NLog.LogLevel`
- **Used by:** [NuGetClient.cs](NuGetClient.cs.md)
