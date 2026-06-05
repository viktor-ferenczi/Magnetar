# Shared/Data/LocalPlugin.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Data` · **Kind:** class (`PluginData` subclass) · **Lines:** 109

## Summary
`LocalPlugin` is the `PluginData` for a pre-compiled plugin DLL sitting on disk (not compiled by Magnetar, not from GitHub). It loads the assembly directly via `Assembly.LoadFile`, inspects the DLL with Mono.Cecil to determine which .NET runtime it targets (so Magnetar can refuse incompatible binaries across its net48/net10 split), and optionally reads an adjacent `<dll>.xml` metadata file using the `GitHubPlugin` XML schema for friendly name, author, platforms, and dependencies.

## Types
### LocalPlugin — class, public : `PluginData`
Wraps a single local `.dll`. `Id` is the DLL filename, `FriendlyName` the filename without extension. The `Runtimes` value is detected from the assembly's references at construction. Loading uses an `AssemblyResolver` rooted at the DLL's directory.

- **Fields:** `Dll` (public DLL path); `github` (`GitHubPlugin` metadata from the sidecar XML); `resolver` (`AssemblyResolver`).
- **Properties:** `IsLocal` => `true`, `IsCompiled` => `false`.
- **Methods:**
  - `LocalPlugin()` — private parameterless ctor (for serialization scenarios).
  - `LocalPlugin(dll)` — sets `Dll`/`Id`/`FriendlyName`, `Status = None`, detects `Runtimes`, and tries to load `<dll>.xml`.
  - `static GetRuntimes(dll)` — reads the assembly with `Mono.Cecil`; returns `"NETCoreApp"` if it references `System.Runtime`, `"NETFramework"` if it references `mscorlib`, else null. This drives `PluginData.IsSupportedRuntime`.
  - `GetAssembly()` — if the DLL exists, builds an `AssemblyResolver` over its folder, allows the DLL file, loads it with `Assembly.LoadFile`, sets `Version`, returns it; otherwise null.
  - `TryLoadDataFile(file)` — XML-deserializes the sidecar expecting a `GitHubPlugin`, copies friendly name, tooltip, author, description, platforms, and dependency ids; logs and swallows errors.
  - `UpdateProfile(draft, enabled)` — base then, if enabled, adds `Id` to `draft.Local`.
  - `GetAssetPath()` — returns the absolute asset folder only if `github.AssetFolder` is set and rooted; else null.
  - `ToString()` => `Id`.

## Cross-references
- **Uses:** `PluginData` (Shared/Data/PluginData.cs); `GitHubPlugin` (Shared/Data/GitHubPlugin.cs) as the XML metadata schema; `AssemblyResolver` (Shared.Network/Core); `Mono.Cecil` (`AssemblyDefinition`); `LogFile` (Shared.Core); `System.Reflection`, `System.Xml.Serialization`.
- **Used by:** [SourcesConfig.cs](../Config/SourcesConfig.cs.md), [PluginList.cs](../PluginList.cs.md)
