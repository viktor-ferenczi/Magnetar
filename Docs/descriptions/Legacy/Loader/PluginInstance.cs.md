# Legacy/Loader/PluginInstance.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Loader` · **Kind:** class · **Lines:** 336

## Summary
Runtime wrapper around a single loaded plugin: it locates the plugin's `IPlugin` implementation type in the compiled assembly, instantiates it, performs reflection-based dependency injection of loader services into well-known static fields/methods, and drives the SE plugin lifecycle (`Init` / `Update` / `HandleInput` / `Dispose`). It also registers the plugin's SE session components (`[MySessionComponentDescriptor]`) and entity/game-logic components (`[MyEntityComponentDescriptor]`) into the live `MySession` / `MyScriptManager`, and on any plugin error it invalidates the plugin's compile cache, marks it errored, and disposes it. Every call into plugin code is wrapped in try/catch so a misbehaving plugin cannot crash the loader.

## Types

### PluginInstance — class, public
Owns one plugin's `IPlugin` object and mediates all interaction with it. Exists to isolate plugin lifecycle, asset loading, SE component registration and error handling from the rest of the loader.

- **Fields:**
  - `mainType` (`Type`, readonly) — the discovered `IPlugin` type.
  - `data` (`PluginData`, readonly) — metadata/config and cache handle for this plugin.
  - `mainAssembly` (`Assembly`, readonly) — the plugin's compiled assembly.
  - `openConfigDialog` (`MethodInfo`) — cached `OpenConfigDialog()` method if present.
  - `plugin` (`IPlugin`) — the live instance (null before instantiation / after dispose).
  - `inputPlugin` (`IHandleInputPlugin`) — `plugin` cast to the input interface, or null.
- **Properties:**
  - `Id`, `FriendlyName`, `Author` — forwarded from `data`.
  - `HasConfigDialog` (`bool`) — true if an `OpenConfigDialog` method was found.
- **Methods:**
  - `PluginInstance(PluginData, Assembly, Type)` (private ctor) — stores the three references; instances are created only via `TryGet`.
  - `ContainsExceptionSite(MemberAccessException)` — Returns true if the exception's `TargetSite` declaring assembly is this plugin's assembly; if so, invalidates the compile cache, logs/marks the error and disposes. Used by `PluginLoader.OnException` to attribute first-chance member-access errors. Caveat noted in code: does not catch exceptions inside transpiled methods or some patches.
  - `Instantiate()` — Runs `DependencyInject`, then `Activator.CreateInstance(mainType)` to the `IPlugin`, casts to `IHandleInputPlugin`, and calls `LoadAssets`. Returns false (and `ThrowError`s) on failure.
  - `DependencyInject()` (private) — Reflection injection into the plugin's static members (legacy Pulsar convention, marked FIXME to migrate to the Pulsar SDK): sets static field `GetConfigPath` to `data.GetConfigPath`, `IsNative` to `Tools.IsNative()`, `PulsarLog` to `LogFile.WriteLine`, and caches the `OpenConfigDialog` method. Each lookup is individually try/caught and merely logged if absent.
  - `LoadAssets()` (private) — If `data.GetAssetPath()` exists, invokes the plugin's `LoadAssets(string)` method with that folder.
  - `OpenConfig()` — Invokes the cached `OpenConfigDialog()` on the live plugin (no-op if not present); `ThrowError`s on failure.
  - `Init(object gameInstance)` — Calls `IPlugin.Init(gameInstance)`; returns false and `ThrowError`s on exception.
  - `RegisterSessionComponents(MySession session)` — For every assembly type marked `[MySessionComponentDescriptor]`, creates a `MySessionComponentBase` and registers it with the session using its `UpdateOrder`/`Priority`. Logs count; `ThrowError`s on failure.
  - `RegisterEntityComponents(MyScriptManager sm)` — For every assembly type marked `[MyEntityComponentDescriptor]` that is a `MyGameLogicComponent`, validates the descriptor's `EntityBuilderType` is a `MyObjectBuilder_Base`, maps it in `sm.TypeToModMap` to `MyModContext.UnknownContext`, and registers it into `sm.EntityScripts` (no subtypes) or `sm.SubEntityScripts` (per subtype name) via `AddEntityScript`. Logs count; `ThrowError`s on failure.
  - `AddEntityScript<T>(Dictionary<T, HashSet<Type>>, T key, Type value)` (private generic) — Adds `value` to the set for `key`, creating the set if needed.
  - `Update()` — Calls `IPlugin.Update()`; returns false if no live plugin.
  - `HandleInput()` — Calls `inputPlugin?.HandleInput()`; returns false if no live plugin.
  - `Dispose()` — Calls `IPlugin.Dispose()` and nulls the plugin/input references; on failure sets `data.Status = PluginStatus.Error` and logs.
  - `ThrowError(string)` (private) — Logs the error, calls `data.Error()`, and disposes the plugin.
  - `TryGet(PluginData, Assembly, out PluginInstance)` (static) — Factory: finds the first `IPlugin`-assignable type in the assembly and constructs a `PluginInstance`. If none, logs and marks `data.Error()`. Catches and details `ReflectionTypeLoadException.LoaderExceptions`.
  - `ToString()` (override) — Delegates to `data.ToString()`.

## Cross-references
- **Uses:** `Pulsar.Shared` (`LogFile`, `Tools`), `Pulsar.Shared.Data` (`PluginData`, `PluginStatus`); SE DS assemblies: `VRage.Plugins` (`IPlugin`, `IHandleInputPlugin`), `Sandbox.Game.World` (`MySession`, `MyScriptManager`), `VRage.Game` / `VRage.Game.Components` (`MySessionComponentDescriptor`, `MyEntityComponentDescriptor`, `MySessionComponentBase`, `MyGameLogicComponent`, `MyModContext`), `VRage.ObjectBuilders` (`MyObjectBuilder_Base`); `HarmonyLib` (`AccessTools`), NLog (`LogLevel`).
- **Used by:** [PluginLoader.cs](PluginLoader.cs.md)
