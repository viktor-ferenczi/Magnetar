# PluginSdkTests/CommandTests.cs

**Project:** PluginSdkTests · **Namespace:** `PluginSdk.Tests` · **Kind:** test class · **Lines:** 470

## Summary
Comprehensive specification for the PluginSdk chat-command pipeline: `CommandRegistry`, `CommandDispatcher`, `CommandModule`, `CommandCaller`, `CommandReply`, `ICommandResponder`, and the associated attributes (`CommandRoot`, `Command`, `Permission`). The tests exercise registration of `CommandModule` subclasses under a `!prefix` root, typed argument binding (scalars, defaults, `params` arrays, quoted strings), nested multi-word command paths (`grid list`), permission gating via `MyPromoteLevel` (SE DS API), `void` handlers that send no reply, broadcast replies, handler-exception reporting, command override (last registration wins), the built-in per-root overview, per-root `help` sub-command, and the global `!help` command — including its filtering by caller permission level. All tests run against a `CapturingResponder` stub; no live SE session is needed.

## Types

### `CommandTests` — class, public

Main Xunit test class. Contains all nested helper types and test methods.

- **Fields/nested types:**
  - `CapturingResponder` (private sealed class) — implements `ICommandResponder`; accumulates `CommandReply` and `CommandCaller` values in lists. Exposes `Texts` (enumerable of reply texts) and `LastText` (last reply text or null) for assertion convenience.
  - `SampleCommands` (public sealed class : `CommandModule`) — annotated with `[CommandRoot("test", "Test Plugin", "a demo plugin")]`; declares commands `ping` (returns `"pong"`), `add` (returns sum of two ints, second defaults to 1), `echo` (returns joined `params string[]`), `yell` (returns `CommandReply.Announce`), `grid list` (nested two-word path), `boom` (always throws `InvalidOperationException`), `secret` (no `[Permission]`, defaults to Admin), `silent` (void handler).
  - `OverrideCommands` (public sealed class : `CommandModule`) — same `"test"` root as `SampleCommands`; registers a conflicting `ping` that returns `"pong2"` to test last-registration-wins.
  - `DefaultCommands` (public sealed class : `CommandModule`) — root `"def"`; has a default command (`Command("")`) and a `sub` subcommand.
  - `AdminDefaultCommands` (public sealed class : `CommandModule`) — root `"adm"`; default command with no `[Permission]` (Admin-only).
  - `HelpPrefixCommands` (public sealed class : `CommandModule`) — root `"help"`; used in the test that proves registering a `"help"` root throws `CommandRegistrationException`.

- **Methods (static helpers):**
  - `BuildDispatcher(out CapturingResponder, Action<string,Exception>) → CommandDispatcher` — registers `SampleCommands` and returns a ready dispatcher.
  - `BuildDispatcherFor(Type, out CapturingResponder) → CommandDispatcher` — single-module variant used for focused tests.
  - `BuildMultiRootDispatcher(out CapturingResponder) → CommandDispatcher` — registers `SampleCommands`, `DefaultCommands`, and `AdminDefaultCommands` for global-help tests.
  - `Caller(MyPromoteLevel) → CommandCaller` — constructs a `CommandCaller` with hard-coded steam id / entity id / name.

- **Test methods (grouped by concern):**
  - Basic dispatch: `Handle_SimpleCommand_RepliesToCaller`, `Handle_NonCommandText_IsNotHandled`, `Handle_UnknownPrefix_IsNotHandled`.
  - Argument binding: `Handle_BindsTypedArgsAndDefaults`, `Handle_MissingRequiredArg_RepliesUsage`, `Handle_BadArgType_RepliesError`, `Handle_ParamsArray_CapturesRemaining`, `Handle_QuotedArgument_KeptTogether`, `Handle_NestedCommandPath_Resolves`.
  - Reply kinds: `Handle_BroadcastReply_IsMarkedBroadcast`, `Handle_VoidHandler_SendsNothing`.
  - Permissions: `Handle_InsufficientPermission_IsDenied`, `Handle_SufficientPermission_RunsAdminCommand`.
  - Default command: `Handle_BarePrefix_WithDefault_RunsDefaultCommand`, `Handle_BarePrefix_WithoutDefault_ShowsOverview`, `Handle_NamedSubcommand_StillResolves_WithDefaultPresent`, `Handle_BarePrefix_DefaultAboveCallerLevel_ShowsOverview`, `Handle_BarePrefix_DefaultAtCallerLevel_RunsIt`.
  - Error handling: `Handle_HandlerException_ReportsAndReplies`, `Handle_UnknownSubcommand_RepliesUnknown`.
  - Registration: `Register_ConflictingCommand_LastRegistrationWins`, `Register_HelpPrefix_IsRejected`.
  - Overview/help: `Overview_HidesCommandsAboveCallerLevel`, `Overview_ShowsAdminCommandsToAdmin`, `Help_ForCommand_ShowsUsage`.
  - Global `!help`: `GlobalHelp_ListsTopLevelCommands_Sorted`, `GlobalHelp_DoesNotListSubcommands`, `GlobalHelp_NonAdmin_HidesRootsWithoutAvailableCommands`, `GlobalHelp_NoAvailableCommands_SaysSo`, `GlobalHelp_WithKnownPrefix_ListsThatRootsSubcommands`, `GlobalHelp_WithUnknownPrefix_FallsBackToTopLevelListing`.

## Cross-references
- **Uses:**
  - `PluginSdk/Commands/` — `CommandModule`, `CommandDispatcher`, `CommandRegistry`, `CommandReply`, `CommandCaller`, `ICommandResponder`, `CommandRegistrationException`, `CommandRoot`, `Command`, `Permission` attributes
  - SE DS assembly (`VRage.Game`) — `MyPromoteLevel` (permission enum)
- **Used by:** _none within the repository_
