# Shared/Preloader.cs

**Project:** Shared · **Namespace:** `Pulsar.Shared` · **Kind:** class · **Lines:** 225

## Summary
Implements Magnetar's "preloader plugin" mechanism: BepInEx/Pulsar-style assembly patching of SE DS DLLs *on disk* before they are loaded into the CLR. A preloader plugin is any assembly containing a public static type named `Preloader` that exposes a `TargetDLLs` string sequence, a `Patch(AssemblyDefinition)` (or `ref AssemblyDefinition`) method, and optional `Initialize`/`Finish` hooks. For each targeted DLL, `Patch` reads the original from the game dir with Mono.Cecil, invokes every plugin's patch method against the `AssemblyDefinition`, forces `ILOnly` (so Cecil can rewrite SE2-style mixed-mode/R2R images), writes the modified copy to a cache dir, and `LoadFrom`s it so the patched version wins resolution. Any failure is fatal (`Environment.Exit(1)`) because the headless server cannot interactively recover from a half-patched runtime.

## Types
### `Preloader` — class, public
Scans the supplied assemblies for `Preloader` types and collects their pre-hooks, post-hooks, and per-DLL patch methods, then applies them on demand.
- **Fields (const):** `ClassName="Preloader"`, `TargetName="TargetDLLs"`, `PatchName="Patch"`, `PreHookName="Initialize"`, `PostHookName="Finish"` — the reflected member names.
- **Fields:** `preHooks` / `postHooks` — sets of static void hook methods; `patches` — map of target DLL file name -> set of patch methods.
- **Properties:** `HasPatches` — true if any patches or hooks were registered.
- **Methods:** `Preloader(IEnumerable<Assembly>)` — finds each assembly's `Preloader` type and registers it; `PreHooks()` / `PostHooks()` — invoke all `Initialize` / `Finish` hooks (each via `SafeInvoke`); `Patch(string gameDir, string cacheDir)` — the core pass: sets up a Cecil `DefaultAssemblyResolver` over the game dir, ensures the cache dir, and for each target DLL: bails if the assembly is already loaded (`EnsureNotLoaded`), reads it (`TryReadAssembly`), applies every patch method, forces `ModuleAttributes.ILOnly`, writes to the cache and `Assembly.LoadFrom`s it; finally deletes stale cache files no longer targeted; `EnsureNotLoaded(string)` — if the DLL is already in the AppDomain, errors, shows a message, and exits (patching a loaded assembly is impossible); `TryReadAssembly(...)` — Cecil read with a fatal-exit `FileNotFoundException` path naming the offending preloader plugins; `AddPreloader(Type)` — validates and registers a preloader type's hooks/patch method (fatal exit if `TargetDLLs` exists but no valid `Patch` method); `GetMethod` — `AccessTools.Method` lookup requiring static/public/void; `GetSequence<T>` — reads a public static `IEnumerable<T>` property; `GetAssemblyName` — formats a method's declaring assembly name; `ApplyPatch(MethodInfo, ref AssemblyDefinition)` — invokes the patch method, propagating the possibly-replaced definition for `ref` signatures; `SafeInvoke(MethodInfo, object[])` — invokes a method, turning any `TargetInvocationException` into a fatal logged exit.

## Cross-references
- **Uses:** `Shared/LogFile.cs`, `Shared/Tools.cs` (`ShowMessage`); Mono.Cecil (`AssemblyDefinition`, `ReaderParameters`, `DefaultAssemblyResolver`, `ModuleAttributes`); Harmony (`AccessTools`); `System.Reflection`; SE DS assemblies (the patch targets).
- **Used by:** [Program.cs](../Legacy/Program.cs.md)
