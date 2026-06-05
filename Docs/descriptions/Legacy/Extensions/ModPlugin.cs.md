# Legacy/Extensions/ModPlugin.cs

**Project:** Legacy · **Namespace:** `Pulsar.Legacy.Extensions` · **Kind:** static class (internal extension methods) + private nested class · **Lines:** 31

## Summary
Extends `ModPlugin` (the Magnetar data type representing a Steam Workshop mod) with the SE DS API objects needed to register a mod with the game engine at runtime. The two extension methods produce a `MyObjectBuilder_Checkpoint.ModItem` and a `MyModContext`, which are the structures the SE DS uses internally to track active mods. This file bridges Magnetar's abstraction layer to the concrete VRage/Sandbox mod registration API.

## Types

### `ModPluginExtensions` — static class, internal
Provides two extension methods on `Pulsar.Shared.Data.ModPlugin` that convert a Magnetar mod descriptor into SE-internal mod representations.

- **Methods:**
  - `GetModItem(this ModPlugin) — creates a MyObjectBuilder_Checkpoint.ModItem keyed by WorkshopId with service "Steam", then calls SetModData with a WorkshopItem wrapping the mod's local folder path; returns the ModItem`
  - `GetModContext(this ModPlugin) — creates a new MyModContext, calls Init twice: first with the ModItem from GetModItem, then with (WorkshopId.ToString(), null, ModLocation); returns the fully initialised context`

### `WorkshopItem` — class, private (nested in `ModPluginExtensions`) : `MyWorkshopItem`
A minimal concrete implementation of the VRage `MyWorkshopItem` base class used solely to carry the mod's local `Folder` path into `SetModData`. No other members are overridden.

- **Fields:** `Folder — set in constructor to the local directory path of the mod`

## Cross-references
- **Uses:** `Shared/Data/ModPlugin.cs` (extended type, provides WorkshopId and ModLocation); SE DS assemblies — `VRage.Game` (`MyObjectBuilder_Checkpoint.ModItem`, `MyModContext`), `VRage.GameServices` (`MyWorkshopItem`)
- **Used by:** _none within the repository_
