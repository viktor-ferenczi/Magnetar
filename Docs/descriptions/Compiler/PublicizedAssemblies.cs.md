# Compiler/PublicizedAssemblies.cs

**Project:** Compiler · **Namespace:** `Pulsar.Compiler` · **Kind:** class · **Lines:** 77

## Summary
Bridges Roslyn source analysis with assembly publicizing. By scanning plugin source for `[assembly: IgnoresAccessChecksTo("...")]` attributes, it determines which referenced SE DS assemblies a plugin wants to access non-publicly, then substitutes a publicized (all members forced public) `MetadataReference` for those assemblies at compile time. This lets plugins legitimately reference internal/private members of Space Engineers / VRage assemblies without the game shipping a separate publicized SDK. One instance is held per `RoslynCompiler`.

## Types
### PublicizedAssemblies — class, public
Collects the set of assembly names a compilation unit declares it ignores access checks to, and caches publicized `MetadataReference`s so each assembly is only rewritten once per compiler instance.
- **Fields:** `ignoredAssemblyNames` — `HashSet<string>` of assembly names harvested from `IgnoresAccessChecksTo` attributes across all loaded sources; `publicizedReferences` — `Dictionary<string, MetadataReference>` cache keyed by assembly name, holding the publicized rewrite of each.
- **Methods:**
  - `InspectSource(SourceText source)` — parses the source with `CSharpSyntaxTree.ParseText`, walks all `AttributeSyntax` nodes whose name ends with `IgnoresAccessChecksTo`, reads the first argument when it is a string literal, and adds that assembly name to `ignoredAssemblyNames`. Called once per `Load` in `RoslynCompiler`.
  - `PublicizeReferenceIfRequired(string targetName, string dependencyName, MetadataReference dependency)` — returns the original reference unchanged unless `dependencyName` is in `ignoredAssemblyNames`; otherwise obtains/creates the publicized reference, logs `Using publicized {dependencyName} for {targetName}`, and returns it. Throws `Exception` if publicizing fails (e.g. the reference has no on-disk file path).
  - `GetPublicizedReference(string referenceName, MetadataReference originalReference, out MetadataReference publicizedRef)` — private; returns cached value if present; otherwise requires the original to be a `PortableExecutableReference` with a non-empty `FilePath`, calls `Publicizer.PublicizeReference`, caches and returns it. Returns `false` (cannot publicize) when there is no backing file.

## Cross-references
- **Uses:** `Compiler/Publicizer.cs` (`Publicizer.PublicizeReference`); `Compiler/LogFile.cs`; Roslyn (`Microsoft.CodeAnalysis` `MetadataReference`/`PortableExecutableReference`, `Microsoft.CodeAnalysis.CSharp` syntax APIs, `SourceText`).
- **Used by:** [RoslynCompiler.cs](RoslynCompiler.cs.md)
