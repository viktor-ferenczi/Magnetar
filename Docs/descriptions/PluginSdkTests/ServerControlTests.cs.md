# PluginSdkTests/ServerControlTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test class · **Lines:** 62

## Summary
Specifies the `ServerControl` static façade that plugins call to trigger server lifecycle operations (save, reload config, quit, restart). The tests confirm two invariants: (1) after `ServerControl.Bind(...)` each public method routes through the corresponding supplied delegate, and (2) calling `ServerControl.Bind(null, null, null, null, null, null)` restores safe no-op defaults — the `bool`-returning operations return `false` and the `void` operations do nothing and do not throw. The façade uses global static state, so each test re-binds without relying on execution order.

## Types

### `ServerControlTests` — class, public

Xunit test class. Contains two test methods.

- **Methods:**
  - `Bind_RoutesEachCallToSuppliedDelegate` — binds six counting-lambda delegates (two returning `bool`, four `void`), invokes all six public methods of `ServerControl`, asserts each delegate was called exactly once and that the `bool` returns are `true`.
    - Methods verified: `ServerControl.SaveWorld()`, `ServerControl.ReloadConfig()`, `ServerControl.SaveAndQuit()`, `ServerControl.SaveAndRestart()`, `ServerControl.QuitWithoutSaving()`, `ServerControl.RestartWithoutSaving()`.
  - `Bind_WithNulls_RestoresSafeNoOps` — first binds real delegates, then immediately binds nulls for all six slots; asserts `SaveWorld()` and `ReloadConfig()` return `false`, and the four void calls complete without exception.

## Cross-references
- **Uses:**
  - `PluginSdk/ServerControl.cs` — `ServerControl.Bind`, `ServerControl.SaveWorld`, `ServerControl.ReloadConfig`, `ServerControl.SaveAndQuit`, `ServerControl.SaveAndRestart`, `ServerControl.QuitWithoutSaving`, `ServerControl.RestartWithoutSaving`
- **Used by:** _none within the repository_
