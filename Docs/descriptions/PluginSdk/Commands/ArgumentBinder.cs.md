# PluginSdk/Commands/ArgumentBinder.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** static class · **Lines:** 155

## Summary
`ArgumentBinder` converts the ordered list of string tokens produced by `CommandLine.Tokenize` into the typed `object[]` array expected by a handler's `MethodInfo.Invoke` call. It handles the full set of scalar types that SE DS plugins realistically use: every integer width, float/double/decimal, bool (with natural aliases `yes`/`no`/`on`/`off`/`1`/`0`), case-insensitive enum names, and a trailing `params` array. Optional parameters (those with a C# default value) are filled from the default when the caller omits them, and a missing required argument yields a descriptive error string rather than an exception.

## Types

### `ArgumentBinder` — static class, internal

Central type-conversion hub called by `CommandDispatcher.ExecuteCommand`. It is kept internal because the conversion rules are an implementation detail of the dispatch pipeline.

- **Methods:**
  - `TryBind(ParameterInfo[] parameters, IReadOnlyList<string> args, out object[] values, out string error) → bool` — Iterates parameters in declaration order. For each `params`-array parameter it consumes all remaining tokens; for each ordinary parameter it converts the next token or, when no token remains, uses the declared default value. Returns `false` and a user-readable `error` string on first failure.
  - `IsParamsArray(ParameterInfo p) → bool` — Returns `true` when the parameter is decorated with `ParamArrayAttribute` and its type is an array; used by both `TryBind` and `RegisteredCommand.BuildSyntax`.
  - `FriendlyTypeName(Type type) → string` — Returns a short English label (`"integer"`, `"number"`, `"true/false"`, `"text"`, enum type name, or the CLR type name as fallback) used in error messages.
  - `TryConvert(string s, Type type, out object value) → bool` (private) — Fast-path branches for `string`, `bool`, all numeric primitives (using `InvariantCulture`), and enums; falls back to `TypeDescriptor.GetConverter` for other types registered via `ComponentModel`.

## Cross-references
- **Uses:** `PluginSdk/Commands/RegisteredCommand.cs` (consumes `ParameterInfo[]`), `System.ComponentModel.TypeDescriptor` (fallback conversion)
- **Used by:** [CommandDispatcher.cs](CommandDispatcher.cs.md), [RegisteredCommand.cs](RegisteredCommand.cs.md)
