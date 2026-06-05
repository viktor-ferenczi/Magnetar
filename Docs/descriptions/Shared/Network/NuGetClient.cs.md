# Shared/Network/NuGetClient.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Network` · **Kind:** class · **Lines:** 248

## Summary
`NuGetClient` wraps the NuGet v3 client SDK to download and extract packages from `api.nuget.org` into a local cache inside Magnetar's data directory. It supports two intake paths: parsing a `packages.config` XML stream (used for plugin-declared config files) and accepting an explicit list of `NuGetPackageId` records (used for programmatic declarations). Before downloading it resolves the full transitive dependency tree using `PackageResolver`, then extracts each package and returns a `NuGetPackage` object pointing to the on-disk install. Harmony (`Lib.Harmony`) is explicitly excluded from downloads because it is already present in the DS64 folder.

## Types

### `NuGetClient` — class, public

Manages the full lifecycle of NuGet package acquisition for one Magnetar instance. It is constructed once per loader run; the constructor loads NuGet's default settings, initialises the extraction context (v3 defaults, XML doc skipping), points at the official NuGet service index, and creates the package folder under `<PulsarDir>/NuGet/packages/`.

The target framework is compile-time selected: `net48` on `NETFRAMEWORK` (Legacy / Windows), `net10.0` otherwise (Interim / Linux).

- **Fields:**
  - `NugetServiceIndex` — constant: `"https://api.nuget.org/v3/index.json"`
  - `ProjectFramework` — static `NuGetFramework` resolved at compile time (`net48` vs `net10.0`)
  - `logger` — static `ILogger` instance (`NuGetLogger`)
  - `packageFolder` — absolute path to the local package cache
  - `sourceRepository` — `SourceRepository` for the official NuGet feed
  - `pathResolver` — `PackagePathResolver` anchored to `packageFolder`
  - `extractionContext` — `PackageExtractionContext` configured for `PackageSaveMode.Defaultv3`, skipping XML docs

- **Methods:**
  - `NuGetClient()` — constructor; loads NuGet settings, creates extraction context, connects to the v3 source repository, creates the package folder, and wires up `pathResolver`
  - `DownloadFromConfig(Stream)` — synchronous entry point: blocks on `DownloadFromConfigAsync` via `Task.Run(...).GetAwaiter().GetResult()`
  - `DownloadFromConfigAsync(Stream)` — reads a `packages.config` stream via `PackagesConfigReader`, iterates the declared packages, and calls `DownloadPackage` for each; returns the collected `NuGetPackage[]`
  - `DownloadPackages(IEnumerable<NuGetPackageId>, bool)` — synchronous entry point for programmatic package lists; wraps `DownloadPackagesAsync`
  - `DownloadPackagesAsync(IEnumerable<NuGetPackageId>, bool)` — converts `NuGetPackageId` records to `PackageIdentity` values, filters out already-installed packages, optionally resolves dependencies via `ResolveDependencies`, then downloads each remaining package; returns `NuGetPackage[]`
  - `ResolveDependencies(IEnumerable<PackageIdentity>, SourceCacheContext)` — builds a `PackageResolverContext` using `DependencyBehavior.Lowest` and the full dependency graph from `GetDependencies`, then calls `PackageResolver.Resolve`
  - `GetDependencies(IEnumerable<PackageIdentity>, SourceCacheContext)` — iterative DFS over the dependency graph using a `Stack<PackageIdentity>`; queries `DependencyInfoResource.ResolvePackage` for each unseen package and pushes its transitive dependencies; returns the flat set of `SourcePackageDependencyInfo`
  - `DownloadPackage(SourceCacheContext, PackageIdentity, NuGetFramework)` — downloads and extracts one package if not already present on disk; falls back to `ProjectFramework` for agnostic/unsupported/null frameworks; uses `DownloadResource` and `PackageExtractor.ExtractPackageAsync`; returns a new `NuGetPackage` or `null`
  - `CheckAlreadyInstalled(string)` — special-cases `Lib.Harmony` (case-insensitive) as already available in DS64; returns `false` for all other IDs

## Cross-references
- **Uses:**
  - `Shared/Network/NuGetLogger.cs` — `NuGetLogger` implements `ILogger` for all SDK calls
  - `Shared/Network/NuGetPackage.cs` — wraps each extracted package directory
  - `Shared/Network/NuGetPackageId.cs` — input DTO with `TryGetIdentity`
  - `Shared/Config/ConfigManager.cs` — reads `ConfigManager.Instance.PulsarDir` for the package folder path
  - External: NuGet client SDK (`NuGet.Common`, `NuGet.Configuration`, `NuGet.Frameworks`, `NuGet.Packaging`, `NuGet.Protocol`, `NuGet.Resolver`)
- **Used by:** [GitHubPlugin.cs](../Data/GitHubPlugin.cs.md), [LocalFolderPlugin.cs](../Data/LocalFolderPlugin.cs.md)
