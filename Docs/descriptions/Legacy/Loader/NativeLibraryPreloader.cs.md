# Legacy/Loader/NativeLibraryPreloader.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Loader` · **Kind:** static class (internal) · **Lines:** 154

## Summary
Linux-only native-library bootstrap that runs once at the very top of `Main()`. It `dlopen`s every bundled `lib*.so*` next to the launcher with an absolute path and `RTLD_GLOBAL` so subsequent symbol lookups resolve from memory instead of disk, then resolves the Windows-style DLL names declared by Magnetar's bundled Steamworks.NET, VRage.EOS and the `se-linux-compat` PE-loader physics wrappers (Havok / RecastDetour / VRage.Native) against those preloaded handles. The resolver is registered via `AssemblyLoadContext.ResolvingUnmanagedDll` on every existing ALC and on every future ALC (through `AppDomain.AssemblyLoad`), so plugins loaded into Magnetar's per-plugin `.pl5` AssemblyLoadContexts inherit native resolution without registering their own resolvers. On non-Linux platforms `Initialize` is a no-op.

## Types

### NativeLibraryPreloader — static class, internal
Centralised native dependency resolver for the Linux DS. Exists because plugin ALCs do not have `baseDir` in their default DllImport probe paths, and several VRage/Steam/EOS DllImport sites spell Windows DLL names that must alias to Linux `.so` files.

- **Fields:**
  - `Handles` (`Dictionary<string, IntPtr>`, ordinal-ignore-case) — maps a `[DllImport]` library name (or alias) to the `dlopen` handle of the underlying `.so`. Populated by preload then by alias materialisation.
  - `HookedContexts` (`HashSet<AssemblyLoadContext>`) — tracks which ALCs already had the resolver hooked, to avoid double subscription.
  - `Aliases` (`(string Alias, string Target)[]`) — static alias table: Steamworks (`steam_api64[.dll]` → `libsteam_api.so`), EOS (`EOSSDK-Shipping[.dll]` → `libEOSSDK-Linux-Shipping.so`), and the se-linux-compat wrappers (`Havok.dll`→`libHavok.so`, `RecastDetour.dll`→`libRecastDetour.so`, `VRage.Native.dll`→`libVRageNative.so`). Targets are looked up in `Handles` after preload.
  - `RTLD_NOW` (const `0x2`), `RTLD_GLOBAL` (const `0x100`) — flags for `dlopen`.
  - `dlopen` (`extern IntPtr(string filename, int flags)`, `[DllImport("libdl.so.2", EntryPoint="dlopen")]`).
- **Methods:**
  - `Initialize(string baseDir)` — Entry point; returns immediately if not Linux. (1) Calls `PreloadBundled(baseDir)`; (2) materialises the alias table by pointing each alias at its target's handle; (3) hooks the resolver on every `AssemblyLoadContext.All` and subscribes to `AppDomain.CurrentDomain.AssemblyLoad` to hook each newly-loaded assembly's ALC.
  - `PreloadBundled(string baseDir)` (private) — Enumerates `lib*.so*` under `baseDir`, `dlopen`s each with absolute path and `RTLD_NOW | RTLD_GLOBAL`, stores the handle under its filename, and additionally aliases the unversioned soname (e.g. `libfoo.so.1` → `libfoo.so`, first-wins). Logs `dlopen` failures to stdout but continues.
  - `StripVersionSuffix(string fileName)` (private) — Truncates everything after the first `.so.` (keeping `.so`), since DllImport sites never spell out soname version metadata.
  - `HookContext(AssemblyLoadContext alc)` (private) — Subscribes `Resolve` to the ALC's `ResolvingUnmanagedDll` event, once per ALC.
  - `Resolve(Assembly assembly, string libraryName)` (private) — `ResolvingUnmanagedDll` handler; returns the cached handle for `libraryName` or `IntPtr.Zero`. Fires only after the runtime's default native probing fails, so the common case bypasses it.

## Cross-references
- **Uses:** BCL `System.Runtime.Loader.AssemblyLoadContext`, `System.Runtime.InteropServices` (`dlopen` via `libdl.so.2`), `System.IO.Directory`; native systems: Linux dynamic loader, Steamworks (`libsteam_api.so`), Epic Online Services (`libEOSSDK-Linux-Shipping.so`), se-linux-compat physics wrappers (`libHavok.so`, `libRecastDetour.so`, `libVRageNative.so`).
- **Used by:** [Program.cs](../Program.cs.md)
