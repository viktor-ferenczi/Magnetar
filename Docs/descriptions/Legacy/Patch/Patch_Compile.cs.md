# Legacy/Patch/Patch_Compile.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 65

## Summary
Postfix-patches `MyScriptCompiler.AnalyzeDiagnostics` to intercept Roslyn compilation failures before they reach SE's own error pipeline. When Pulsar logging is active the patch clears SE's native `messages` list (preventing double-reporting via `Patch_MyDefinitionErrors`) and instead accumulates structured diagnostic strings — including file name and source position — into a static `Diagnostics` set. File paths are cleaned so only the workshop-relative portion is shown (cross-platform, handles both Windows backslash and Linux forward-slash layouts), hiding the host machine's full filesystem path.

## Types

### Patch_Compile — static class, public
Harmony Postfix on `VRage.Scripting.MyScriptCompiler.AnalyzeDiagnostics`. Applied in the `"Early"` patch category. When `PulsarLog` is `true` and the overall compilation result is a failure, the patch takes control of diagnostic reporting: it clears the `messages` list that would normally drive `Patch_MyDefinitionErrors.Add`, then walks the Roslyn `ImmutableArray<Diagnostic>` to find errors and warnings-as-errors, formats each one into `"<id>: <message> in file: <path> (line,col)"` form, and stores the strings in `Diagnostics`.

- **Fields:** `PulsarLog — bool; true while a client-mod compilation is in progress (toggled by Patch_MyDefinitionErrors.RedirectModLogging)` · `Diagnostics — HashSet<string>; accumulated Roslyn diagnostic messages for the current compilation run`
- **Methods:** `Postfix(ImmutableArray<Diagnostic> diagnostics, List<Message> messages, ref bool success) — Harmony Postfix; no-ops when compilation succeeded or logging is inactive, otherwise clears messages and collects structured diagnostics` · `CleanFilePath(string path) — private; strips the host-machine prefix from a workshop source-file path, keeping only the path starting from the SE app-id segment (244850) minus the first five path segments`

## Cross-references
- **Uses:** `Shared/Steam.cs` (for `Steam.AppIdSe1` = 244850 in path stripping), `Legacy/Patch/Patch_MyDefinitionErrors.cs` (comment: cleared messages prevent it from triggering), Roslyn (`Microsoft.CodeAnalysis.Diagnostic`/`DiagnosticSeverity`), `VRage.Scripting.MyScriptCompiler` (patched target)
- **Used by:** [Patch_MyDefinitionErrors.cs](Patch_MyDefinitionErrors.cs.md)
