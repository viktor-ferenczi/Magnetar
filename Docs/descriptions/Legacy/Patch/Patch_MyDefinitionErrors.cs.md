# Legacy/Patch/Patch_MyDefinitionErrors.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 40

## Summary
Prefix-patches `MyDefinitionErrors.Add` to intercept Roslyn compilation-failure error messages and redirect them to Magnetar's own log, replacing SE's raw, path-cluttered error string with a cleaner structured output that pairs the mod name with the per-diagnostic messages already collected by `Patch_Compile`. Acts as the second half of a two-patch tandem: `Patch_Compile` collects Roslyn diagnostics; this patch formats and logs them, then suppresses the original `Add` call so SE's default error pipeline is bypassed.

## Types

### Patch_MyDefinitionErrors — static class, public
Harmony Prefix on `VRage.Game.MyDefinitionErrors.Add(MyModContext, string, ...)`, applied in the `"Early"` patch category. When `PulsarLog` is `true` and the incoming `message` contains the string `"Compilation"`, the prefix:

1. Strips the SE path prefix `"Compilation of <ModsPath><sep><ModId>_"` and the trailing `" failed:"` from the message using `Tools.RemoveAll`, leaving only the human-readable mod/file name.
2. Logs `"Failed to build <name>:"` via `LogFile.Error`.
3. Iterates `Patch_Compile.Diagnostics` and logs each structured Roslyn diagnostic line.
4. Returns `false` to suppress the original `MyDefinitionErrors.Add`.

For non-compilation messages the prefix returns `true` (lets SE handle them normally). The static helper `RedirectModLogging(bool)` synchronously toggles both `PulsarLog` and `Patch_Compile.PulsarLog` so both patches switch together around a client-mod compilation run.

- **Fields:** `PulsarLog — bool; enables Pulsar-side compilation error logging; toggled by RedirectModLogging`
- **Methods:** `Prefix(MyModContext context, string message) — Harmony Prefix; intercepts compilation-failure errors, logs structured output, returns false to suppress original` · `RedirectModLogging(bool enabled) — sets PulsarLog and Patch_Compile.PulsarLog together; called by Patch_MyScriptManager around each mod-script load`

## Cross-references
- **Uses:** `Legacy/Patch/Patch_Compile.cs` (`Patch_Compile.Diagnostics`, `Patch_Compile.PulsarLog`), `Shared/Tools.cs` (`Tools.RemoveAll`), `Shared/LogFile.cs` (`LogFile.Error`), `VRage.Game.MyDefinitionErrors.Add` (patched target), `VRage.FileSystem.MyFileSystem.ModsPath`
- **Used by:** [Patch_MyScriptManager.cs](Patch_MyScriptManager.cs.md)
