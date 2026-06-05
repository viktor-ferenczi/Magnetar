# Legacy/Paths/PathResolverBinder.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Paths` · **Kind:** static class (internal) · **Lines:** 77

## Summary
Wires the `PluginSdk.Paths.PathResolver` facade to the LinuxCompat plugin's case-insensitive path cache at startup. On Linux, the SE DS is case-sensitive, so the LinuxCompat plugin (`LinuxCompatServer.dll`) maintains a cache of real on-disk paths. `PathResolverBinder.Bind()` discovers that plugin's internal `PathHelpers` and `PathCache` types by reflection, constructs a `ReflectionPathResolver` backend from them, and installs it via `PathResolver.Install`. On Windows (or when LinuxCompat is absent), the call is a safe no-op: the SDK shim remains in place and all path operations pass through unchanged.

## Types

### `PathResolverBinder` — static class, internal
Called once at launcher startup to connect the path-resolution SDK to the LinuxCompat plugin. All failure paths are caught and logged rather than propagated, so a missing or renamed LinuxCompat assembly never crashes the server.

- **Fields:** `AssemblyName — const string "LinuxCompatServer", the expected assembly name for the fast-path lookup`; `PathHelpersTypeName — const string full name of the PathHelpers type inside LinuxCompat`; `PathCacheTypeName — const string full name of the PathCache type inside LinuxCompat`
- **Methods:**
  - `Bind() — public static; iterates loaded assemblies to find PathHelpers and PathCache types via FindType, calls ReflectionPathResolver.TryCreate, and if successful installs the backend with PathResolver.Install; all exceptions caught and logged`
  - `FindType(fullName) — private static; scans AppDomain.CurrentDomain.GetAssemblies() in two passes: first preferring the assembly named "LinuxCompatServer" (fast path), then falling back to all assemblies; returns null if not found`

## Cross-references
- **Uses:** `Legacy/Paths/ReflectionPathResolver.cs` (`TryCreate` factory); `PluginSdk/Paths/PathResolver.cs` (`Install` method); `Shared/LogFile.cs` (logging); LinuxCompat plugin (`LinuxCompatServer.dll`, `ServerPlugin.Patches.PathHandling.PathHelpers`, `ServerPlugin.Patches.PathHandling.PathCache`) — discovered at runtime via reflection
- **Used by:** [PluginLoader.cs](../Loader/PluginLoader.cs.md)
