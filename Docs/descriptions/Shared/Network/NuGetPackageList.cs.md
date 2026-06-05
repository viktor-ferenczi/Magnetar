# Shared/Network/NuGetPackageList.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared.Network` · **Kind:** class · **Lines:** 20

## Summary
`NuGetPackageList` is a compact container that carries a plugin's NuGet dependency declaration in two optional forms: a path to a `packages.config` file (`Config`) and/or an inline array of `NuGetPackageId` records (`PackageIds`). It is serialised with ProtoBuf for binary IPC and with XML for `<PackageReference>` elements in plugin manifests. `NuGetClient` consumes it to decide which download path to follow.

## Types

### `NuGetPackageList` — class, public

- **Properties:**
  - `Config` — `[ProtoMember(1)]`; relative path (any separator) to a `packages.config` file bundled with the plugin; `null` or empty when unused
  - `PackageIds` — `[ProtoMember(2)]`, `[XmlElement("PackageReference")]`; zero or more inline package ID/version pairs; `null` when unused
  - `PackagesConfigNormalized` — computed; returns `Config` with backslashes replaced by forward slashes and any leading slash stripped, producing a portable relative path suitable for cross-platform file lookup
  - `HasPackages` — computed; `true` when either `Config` is non-empty or `PackageIds` contains at least one entry; used as a guard before invoking `NuGetClient`

## Cross-references
- **Uses:**
  - `Shared/Network/NuGetPackageId.cs` — element type of `PackageIds`
  - External: `ProtoBuf` (`ProtoContract`, `ProtoMember`)
  - External: `System.Xml.Serialization` (`XmlElement`)
- **Used by:** [GitHubPlugin.cs](../Data/GitHubPlugin.cs.md), [LocalFolderPlugin.cs](../Data/LocalFolderPlugin.cs.md)
