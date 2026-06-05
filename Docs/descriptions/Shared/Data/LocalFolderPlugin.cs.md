# Shared/Data/LocalFolderPlugin.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Data` · **Kind:** class (`PluginData` subclass) · **Lines:** 334

## Summary
`LocalFolderPlugin` is the developer-facing `PluginData` that compiles a plugin from a local source folder on every launch (no GitHub download, no cache). It enumerates the folder's tracked `.cs` files — preferring `git ls-files` so build artifacts and ignored files are skipped, falling back to a recursive scan — compiles them via Magnetar's `ICompiler`, optionally restores NuGet dependencies described by an accompanying GitHub-style `.xml` data file, and loads the assembly in-memory with PDB symbols for debugging. It is how Magnetar runs plugins under active development.

## Types
### LocalFolderPlugin — class, public : `PluginData`
Represents a plugin built from a working directory. Its `Id`/`FriendlyName` derive from the folder name. An associated `GitHubPlugin` data file (deserialized from XML) supplies metadata, source directories, NuGet references, and asset folder. Recompiles fresh each `GetAssembly`.

- **Fields:** `GitTimeout`=10000ms const; `sourceDirectories` (restriction filter); `github` (`GitHubPlugin` metadata loaded from the data file); `resolver` (`AssemblyResolver`); `settings` (`LocalFolderConfig`); `Folder` (public source folder path).
- **Properties:** `IsLocal` => `true`, `IsCompiled` => `true`.
- **Methods:**
  - `LocalFolderPlugin(folder)` — sets `Id`/`FriendlyName`/`Folder`, default `settings`, `Status = None`.
  - `ToString()` => `Id`.
  - `LoadData(config)` — resolves the data file path (rooted or relative to `Folder`), deep-copies the `LocalFolderConfig`, and calls `DeserializeFile`.
  - `GetAssembly()` — validates the folder exists, creates a (debug-aware) compiler, installs NuGet dependencies if the data file declares packages, gathers project files (git or fallback), loads each into the compiler (passing the file path for embedded PDB when debug), then `Compile` and `Assembly.Load(data, symbols)`. Throws if the folder is missing or no `.cs` files found. Sets `Version`.
  - `InstallDependencies(compiler)` — builds a per-folder NuGet bin dir under `<PulsarDir>/NuGet/bin/<folderHash>` (recreated each time), restores packages from a `packages.config` and/or package id list, installs them, and wires an `AssemblyResolver` to that bin dir.
  - `static InstallPackage(package, compiler, binDir)` — copies lib and content files into `binDir`; adds top-level lib files as compiler dependencies.
  - `GetProjectFilesGit(folder)` — runs `git ls-files --cached --others --exclude-standard` in the folder (10s timeout, killed on overrun), returns existing valid `.cs` files; logs git stderr and returns null on non-zero exit or exception so the caller falls back.
  - `GetProjectFilesFallback(folder)` — warns, then recursively enumerates `.cs` files excluding `bin`/`obj` and applying `IsValidProjectFile`.
  - `IsValidProjectFile(file)` — true if no `sourceDirectories` restriction, else the file is under one of them.
  - `UpdateProfile(draft, enabled)` — base then, if enabled, adds `LocalFolderConfig { Id }` to `draft.DevFolder`.
  - `DeserializeFile(file)` — null file clears all metadata; otherwise XML-deserializes the data file expecting a `GitHubPlugin`, calls `InitPaths`, copies metadata (friendly name, tooltip, author, runtimes, platforms, dependencies), computes absolute `sourceDirectories`, stores the relative data-file path in `settings`, and keeps the `github` metadata object. Logs and swallows errors.
  - `GetAssetPath()` — absolute path of the folder's asset directory (from `github.AssetFolder`), or null.

## Cross-references
- **Uses:** `PluginData` (Shared/Data/PluginData.cs); `GitHubPlugin` (Shared/Data/GitHubPlugin.cs) as the XML metadata schema; `LocalFolderConfig`/`PluginDataConfig` (Shared.Config); `ICompiler`, `Tools.Compiler`, `Tools.IsNative` (Compiler / Roslyn); `NuGetClient`/`NuGetPackage`/`NuGetPackageList` (Shared.Network — NuGet); `AssemblyResolver` (Shared.Network/Core); `ConfigManager`, `LogFile`, `PluginProgress` (Shared.Config/Core); `System.Diagnostics.Process` (the `git` CLI).
- **Used by:** [PluginList.cs](../PluginList.cs.md)
