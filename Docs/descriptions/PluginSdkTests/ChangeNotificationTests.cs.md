# PluginSdkTests/ChangeNotificationTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test class · **Lines:** 257

## Summary
Verifies the change-notification contract of `PluginConfig` — the base class for all Magnetar plugin configuration objects. The tests prove that scalar property assignments raise `INotifyPropertyChanged.PropertyChanged` only when the new value differs from the old one, that in-place mutation of `List<T>` or `SerializableDictionary<TKey, TValue>` instances is invisible to the notification system (because reference equality is unchanged), and that the documented escape hatch `PluginConfig.NotifyChanged(string)` bypasses the equality gate and fires the event unconditionally. This file is purely a specification for the PluginSdk.Config subsystem; it carries no production code.

## Types

### `ChangeNotificationTests` — class, public

Xunit test class covering `PluginConfig` property-change notification. It creates `TestConfig` instances (the canonical fixture config defined in `TestConfig.cs`), wires a listener via the private helper `CaptureChanges`, mutates properties, and asserts which `PropertyChanged` events were raised and in what order.

- **Methods:**
  - `CaptureChanges(INotifyPropertyChanged) → List<string>` — attaches a lambda to `PropertyChanged` and returns the accumulator list; shared by all tests.
  - `Bool_SettingDifferentValue_RaisesEvent` / `Bool_SettingSameValue_DoesNotRaiseEvent` — verifies equality gating for `bool`.
  - `Int_SettingDifferentValue_RaisesEvent` / `Long_SettingDifferentValue_RaisesEvent` / `Float_SettingDifferentValue_RaisesEvent` / `Double_SettingDifferentValue_RaisesEvent` — same contract for `int`, `long`, `float`, `double`.
  - `String_SettingDifferentValue_RaisesEvent` / `String_SettingSameValue_DoesNotRaiseEvent` — equality gating for `string`.
  - `List_Reassignment_RaisesEvent` — assigning a new `List<int>` instance raises the event.
  - `List_InPlaceMutation_DoesNotRaiseEvent` — calling `Add` on the existing list instance does not raise the event; documents the limitation.
  - `List_DocumentedMutationPattern_RaisesEvent` — mutate in place then call `NotifyChanged` to explicitly fire; asserts the documented workaround.
  - `Dict_Reassignment_RaisesEvent` / `Dict_InPlaceMutation_DoesNotRaiseEvent` / `Dict_DocumentedMutationPattern_RaisesEvent` — same three scenarios for `SerializableDictionary<string, int>`.
  - `Struct_Reassignment_WithDifferentValue_RaisesEvent` / `Struct_Reassignment_WithSameValue_DoesNotRaiseEvent` — value-type equality gating for `TestStruct`.
  - `ListOfStruct_Reassignment_RaisesEvent` / `ListOfStruct_InPlaceAdd_DoesNotRaiseEvent` — list of structs follows the same referential rules as list of scalars.
  - `NotifyChanged_AfterInPlaceMutation_RaisesEventForNamedProperty` — `NotifyChanged` triggers the event for the supplied property name regardless of actual state.
  - `NotifyChanged_NotifiesUnconditionally_EvenWithoutAChange` — `NotifyChanged` has no equality gate; it always fires.
  - `NotifyChanged_AfterNestedInPlaceMutation_RaisesEventForTopLevelProperty` — a struct member sharing a list reference with the original can be notified via the top-level property name.

## Cross-references
- **Uses:**
  - `PluginSdkTests/TestConfig.cs` — `TestConfig`, `TestStruct`, `NestedStruct` (fixtures)
  - `PluginSdk/Config/PluginConfig.cs` — `PluginConfig`, `NotifyChanged`
  - `PluginSdk/Tools/SerializableDictionary.cs` — `SerializableDictionary<TK,TV>`
- **Used by:** _none within the repository_
