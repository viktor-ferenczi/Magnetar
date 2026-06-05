# Legacy/Paths/ReflectionPathResolver.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Paths` · **Kind:** sealed class (internal) : `IPathResolver` · **Lines:** 94

## Summary
An `IPathResolver` backend that forwards path operations to the LinuxCompat plugin's `PathHelpers` and `PathCache` static methods via pre-bound delegates. Reflection cost is incurred exactly once in `TryCreate` when the delegates are created with `Delegate.CreateDelegate`; every subsequent call on the hot path is a plain delegate invocation with no further reflection overhead. The class is constructed only by `PathResolverBinder` and is never exposed outside the `Legacy.Paths` subsystem.

## Types

### `ReflectionPathResolver` — sealed class, internal : `IPathResolver`
Holds six strongly-typed delegates bound to the corresponding static methods on the LinuxCompat types. The private constructor is called only from `TryCreate`, which returns `null` if any required method is missing, ensuring the backend is installed only when fully operational.

- **Fields:**
  - `_normalize — Func<string,string> bound to PathHelpers.Normalize`
  - `_toWindowsPath — Func<string,string> bound to PathHelpers.ToWindowsPath`
  - `_getFileName — Func<string,string> bound to PathHelpers.GetFileName`
  - `_getFileNameWithoutExtension — Func<string,string> bound to PathHelpers.GetFileNameWithoutExtension`
  - `_resolveContentFilePath — Func<string,string,string> bound to PathHelpers.ResolveContentFilePath`
  - `_resolveAbsolute — Func<string,string> bound to PathCache.ResolveAbsolute`
- **Methods:**
  - `TryCreate(pathHelpers, pathCache) — public static factory; calls Bind1/Bind2 for all six operations and returns a new ReflectionPathResolver if all succeed, or null if any method is absent or has the wrong return type`
  - `Bind1(type, name) — private static; locates a public static method with one string parameter and string return type, creates and returns a Func<string,string> delegate, or null`
  - `Bind2(type, name) — private static; same for two-string-parameter methods, returns Func<string,string,string>`
  - `Normalize(path) — IPathResolver impl; delegates to _normalize`
  - `ToWindowsPath(path) — IPathResolver impl; delegates to _toWindowsPath`
  - `GetFileName(path) — IPathResolver impl; delegates to _getFileName`
  - `GetFileNameWithoutExtension(path) — IPathResolver impl; delegates to _getFileNameWithoutExtension`
  - `ResolveContentFilePath(relativePath, rootPath) — IPathResolver impl; delegates to _resolveContentFilePath`
  - `ResolveAbsolute(absolutePath) — IPathResolver impl; delegates to _resolveAbsolute`

## Cross-references
- **Uses:** `PluginSdk/Paths/IPathResolver.cs` (implemented interface); LinuxCompat plugin (`PathHelpers`, `PathCache` types, discovered externally by `PathResolverBinder`)
- **Used by:** [PathResolverBinder.cs](PathResolverBinder.cs.md)
