# Legacy/Patch/Patch_DedicatedServerRun.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Patch` · **Kind:** static class · **Lines:** 78

## Summary
Transpiler-patches `VRage.Dedicated.DedicatedServer.Run` to remove the Telerik/WinForms configuration UI and the Windows Service branch, replacing the entire method body with a minimal headless startup sequence. Magnetar is configured externally through a Web UI, so neither the interactive configurator (`SelectInstanceForm` / `ConfigForm`) nor `WindowsService` is needed. The patch is resolved purely by reflection so the file has no compile-time dependency on `VRage.Dedicated` (whose internal members are inaccessible) and avoids pulling in `System.ServiceProcess`.

## Types

### Patch_DedicatedServerRun — static class, internal
Harmony Transpiler on `VRage.Dedicated.DedicatedServer.Run`, matched by string `"VRage.Dedicated.DedicatedServer, VRage.Dedicated"` and applied in the `"Early"` patch category. The transpiler discards all original IL and emits a compact replacement:

1. `InitializeServices = initializeServices;` — stores the service-initialization delegate (arg 1) via its private property setter.
2. `if (ProcessArgs(args)) return;` — honours command-line flags such as `-console`, `-path`, `-session:`, etc.; if `ProcessArgs` already handled the launch, the method returns immediately.
3. `RunMain(DefaultInstanceName, null, isService: false, showConsole: true, checkAlive: false)` — runs the server against the default instance, loading the most recent world from `LastSession.sbl` exactly as the UI path would.

All three target members (`InitializeServices` setter, `ProcessArgs`, `RunMain`, `DefaultInstanceName` field) are resolved at patch-time via `AccessTools` reflection on the declaring type so no direct type reference is needed.

- **Methods:** `Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) — Harmony Transpiler; emits replacement IL for DedicatedServer.Run that skips all UI/service branches and proceeds directly to RunMain`

## Cross-references
- **Uses:** `VRage.Dedicated.DedicatedServer` (patched target; accessed by name string), HarmonyLib `AccessTools`/`CodeInstruction`/`ILGenerator`
- **Used by:** _none within the repository_
