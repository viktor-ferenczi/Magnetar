# PluginSdkTests/PathResolverTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test class · **Lines:** 88

## Summary
Specifies the `PathResolver` façade and its `IPathResolver` plug-in point. On Windows (and any case-insensitive filesystem) the default shim passes all paths through unchanged, delegating to `System.IO.Path` BCL helpers where appropriate. On Linux the host installs a case-insensitive resolver (`IPathResolver` implementation) that maps game-content relative paths to their real on-disk form despite case mismatches. These tests verify: (1) that the shim is installed after `PathResolver.Install(null)` and that `IsCaseInsensitiveResolverActive` reports `false`; (2) that the shim is transparent for `Normalize`, `ToWindowsPath`, `ResolveAbsolute`, and produces BCL-equivalent results for `GetFileName` and `GetFileNameWithoutExtension`; (3) that `ResolveContentFilePath` with a non-empty root combines via `Path.Combine`; and (4) that installing a real `IPathResolver` causes every façade method to forward to the backend and sets `IsCaseInsensitiveResolverActive` to `true`.

## Types

### `PathResolverTests` — class, public

Xunit test class. Contains one private stub type and four test methods.

- **Nested types:**
  - `FakeResolver` (private sealed class : `IPathResolver`) — records the name of the last method called in `LastCall` and prefixes each return value with a single-letter marker (`N:`, `W:`, `F:`, `E:`, `C:`, `A:`) so tests can assert which backend method was reached without inspecting global state.

- **Methods:**
  - `Shim_IsActiveByDefault` — calls `PathResolver.Install(null)` to reset state, asserts `PathResolver.IsCaseInsensitiveResolverActive == false`.
  - `Shim_PassesPathsThroughUnchanged` — after reset, asserts that `Normalize`, `ToWindowsPath`, and `ResolveAbsolute` return their input unchanged.
  - `Shim_GetFileName_MatchesBcl` — asserts that `PathResolver.GetFileName` and `PathResolver.GetFileNameWithoutExtension` match `System.IO.Path` results exactly.
  - `Shim_ResolveContentFilePath_CombinesWithRoot` — empty root returns the relative path unmodified; non-empty root uses `Path.Combine`.
  - `Install_RoutesCallsToBackend_AndReportsActive` — installs `FakeResolver`, asserts `IsCaseInsensitiveResolverActive == true`, checks each method returns the marker-prefixed result, then restores the shim in a `finally` block.

## Cross-references
- **Uses:**
  - `PluginSdk/Paths/PathResolver.cs` — `PathResolver`, `IPathResolver`
- **Used by:** _none within the repository_
