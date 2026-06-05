# PluginSdk/Paths/ShimPathResolver.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Paths` · **Kind:** class · **Lines:** 36

## Summary

Default, no-op implementation of `IPathResolver` used when the server is running on a case-insensitive filesystem (Windows) or when no real case-insensitive backend has been installed yet. Every method is a trivial pass-through to `System.IO.Path` or the input string, so it adds no overhead. Its presence ensures that `PathResolver` is always in a valid, usable state from the moment the SDK loads, without requiring the host to call `Install` before plugins access path utilities.

## Types

### `ShimPathResolver` — sealed class, internal : `IPathResolver`

Satisfies the `IPathResolver` contract with Windows-correct (case-insensitive filesystem) semantics. Declared `internal sealed` so only the SDK itself and the host (via `PathResolver.Install`) can reference it.

- **Methods:**
  - `Normalize(string path) → string` — returns `path` unchanged (no normalization needed on Windows)
  - `ToWindowsPath(string path) → string` — returns `path` unchanged (already Windows-shaped on Windows)
  - `GetFileName(string path) → string` — delegates to `System.IO.Path.GetFileName`; returns the input as-is if `null` or empty
  - `GetFileNameWithoutExtension(string path) → string` — delegates to `System.IO.Path.GetFileNameWithoutExtension`; guards against `null`/empty
  - `ResolveContentFilePath(string relativePath, string rootPath) → string` — returns `relativePath` unchanged if it is `null`/empty or already rooted; otherwise combines `rootPath + relativePath` via `Path.Combine`; ignores `rootPath` if it is `null`/empty
  - `ResolveAbsolute(string absolutePath) → string` — returns `absolutePath` unchanged (casing is already canonical on Windows)

## Cross-references

- **Uses:** `System.IO.Path` (BCL), `PluginSdk/Paths/IPathResolver.cs`
- **Used by:** [PathResolver.cs](PathResolver.cs.md)
