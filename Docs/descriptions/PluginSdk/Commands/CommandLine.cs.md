# PluginSdk/Commands/CommandLine.cs

**Project:** PluginSdk · **Namespace:** `PluginSdk.Commands` · **Kind:** static class · **Lines:** 69

## Summary
`CommandLine` provides a single `Tokenize` method that splits a raw chat string (with the leading `!` already stripped) into an ordered `List<string>` of tokens. It supports double-quoted groups so arguments containing spaces (e.g., player names) can be passed as a single token, and supports `\` as a universal escape character both inside and outside quotes. This is the first stage of the dispatch pipeline, called by `CommandDispatcher.Handle`.

## Types

### `CommandLine` — static class, internal

Stateless single-method utility. The parser is a single-pass character loop maintaining three boolean flags (`inToken`, `inQuotes`, `escape`) and a `StringBuilder` accumulator. On whitespace outside quotes the current token is flushed; a trailing non-empty token (no trailing space) is also flushed after the loop. An empty input returns an empty list.

- **Methods:**
  - `Tokenize(string input) → List<string>` — Splits `input` on unquoted whitespace. `"multi word"` is yielded as one token with quotes removed. `\` escapes the next character unconditionally. Returns an empty list for null or empty input.

## Cross-references
- **Uses:** (standard library only — `System.Text.StringBuilder`, `System.Collections.Generic.List<string>`)
- **Used by:** [CommandDispatcher.cs](CommandDispatcher.cs.md)
