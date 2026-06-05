# PluginSdk/Logging/LogJson.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Logging` · **Kind:** internal static class · **Lines:** 51

## Summary
Centralises `System.Text.Json` configuration and serialization helpers so both `MagnetarLogSink` and `QuasarLogSink` produce identical JSON shapes for the optional structured `data` payload. Serialization is unconditionally safe: any exception thrown by the serializer is caught and replaced with a small error object `{ error, type }`, so a badly-formed or cyclically-referenced payload can never crash the logging path.

## Types

### `LogJson` — internal static class
Owns the shared `JsonSerializerOptions` instance and exposes two serialization helpers.

- **Fields:** `Options : JsonSerializerOptions` — shared options: `DefaultIgnoreCondition = WhenWritingNull` (drops absent/null fields from output), `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping` (keeps `+`, `<`, `&` unescaped for readability), `WriteIndented = false` (compact single-line output)
- **Methods:**
  - `Serialize(object data) : string` — returns a compact JSON string; on serializer failure returns a `{ "error": "...", "type": "..." }` JSON string
  - `ToElement(object data) : JsonElement` — serializes to a `JsonElement` so the result can be embedded as a nested value in a parent JSON object (used by `QuasarLogSink`); applies the same error fallback
  - `Error(object data, Exception ex) : object` (private) — builds the anonymous fallback object

## Cross-references
- **Uses:** `System.Text.Json` (BCL), `System.Text.Encodings.Web` (BCL)
- **Used by:** [MagnetarLogSink.cs](MagnetarLogSink.cs.md), [QuasarLogSink.cs](QuasarLogSink.cs.md)
