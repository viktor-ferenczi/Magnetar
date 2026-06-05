# PluginSdk/Logging/LogEnvironment.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Logging` · **Kind:** static class · **Lines:** 39

## Summary
Acts as the environment probe that decides which `ILogSink` the SDK uses. When the Quasar Agent launches a managed server process it injects the `QUASAR_AGENT` environment variable; `LogEnvironment` detects that variable and selects `QuasarLogSink`. When the variable is absent (standalone Magnetar), `MagnetarLogSink` is chosen instead. This one check is the only coupling point between the logging subsystem and the deployment topology.

## Types

### `LogEnvironment` — static class, public
Provides two factory helpers used by `Logger.Create`.

- **Fields:** `QuasarEnvironmentVariable = "QUASAR_AGENT"` — the environment variable name the Quasar Agent sets on managed server processes
- **Methods:**
  - `IsManagedByQuasar() : bool` — returns true when `QUASAR_AGENT` is set to a non-empty string
  - `CreateDefaultSink() : ILogSink` — returns a new `QuasarLogSink` if managed by Quasar, otherwise a new `MagnetarLogSink`

## Cross-references
- **Uses:** `PluginSdk/Logging/ILogSink.cs`, `PluginSdk/Logging/MagnetarLogSink.cs`, `PluginSdk/Logging/QuasarLogSink.cs`
- **Used by:** [Logger.cs](Logger.cs.md), [LoggingTests.cs](../../PluginSdkTests/LoggingTests.cs.md)
