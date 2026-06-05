# Compiler/Publicizer.cs

**Project:** Compiler · **Namespace:** `Pulsar.Compiler` · **Kind:** static class · **Lines:** 151

## Summary
Performs the actual IL-level publicizing of an SE DS assembly using Mono.Cecil: it reads the assembly from disk, forces every non-public type, field, method, and property to public, and re-emits it to an in-memory `MetadataReference` for Roslyn. This is what makes plugins able to call internals of Space Engineers / VRage assemblies they declared via `IgnoresAccessChecksTo`. Compiler-generated members are skipped, and virtual methods are left untouched (except via property accessors) to avoid breaking the vtable / override semantics.

## Types
### Publicizer — static class, internal
Stateless utility. The public entry point reads an assembly via Cecil and returns a rewritten reference; the private helpers walk and mutate accessibility flags on each member kind.
- **Methods:**
  - `PublicizeReference(PortableExecutableReference reference)` — public entry; reads the assembly at `reference.FilePath` with a `ReaderParameters` whose `AssemblyResolver` is the shared `RoslynReferences.Instance.Resolver`; calls `PublicizeAssembly`; ORs `ModuleAttributes.ILOnly` into `MainModule.Attributes` (needed because SE2 ships mixed-mode assemblies that Cecil otherwise refuses to write — the native parts are irrelevant for publicizing); writes the assembly to a `MemoryStream` and returns `MetadataReference.CreateFromStream`.
  - `PublicizeAssembly(AssemblyDefinition assembly)` — private; iterates every module and `module.GetTypes()` (includes nested), publicizing each type, then all of its fields, methods, and properties.
  - `TryPublicizeType(TypeDefinition type)` — private; skips compiler-generated and already-public types; for nested types (`IsNested`/`IsNestedAssembly`/`IsNestedFamilyOrAssembly`/`IsNestedFamilyAndAssembly`) sets `IsNestedPublic`, otherwise sets `IsPublic`. Returns whether a change was attempted.
  - `TryPublicizeField(FieldDefinition field)` — private; skips compiler-generated; promotes private/assembly/family/family-or-assembly/family-and-assembly fields to `IsPublic`.
  - `TryPublicizeMethod(MethodDefinition method, bool force = false)` — private; unless `force`, skips compiler-generated and virtual methods (virtuals are not flattened to public to preserve overriding); promotes the same non-public visibility set to `IsPublic`.
  - `TryPublicizeProperty(PropertyDefinition property)` — private; skips compiler-generated; force-publicizes the getter and setter `MethodDefinition`s (passing `force: true`, so even virtual accessors are publicized).
  - `IsCompilerGenerated(IMemberDefinition member)` — private; true if the member carries `System.Runtime.CompilerServices.CompilerGeneratedAttribute`.

## Cross-references
- **Uses:** Mono.Cecil (`AssemblyDefinition`, `TypeDefinition`, `FieldDefinition`, `MethodDefinition`, `PropertyDefinition`, `ModuleAttributes`, `ReaderParameters`); `Compiler/RoslynReferences.cs` (shared `Resolver`); Roslyn (`MetadataReference`, `PortableExecutableReference`); SE DS / VRage assemblies (the publicizing targets).
- **Used by:** [PublicizedAssemblies.cs](PublicizedAssemblies.cs.md)
