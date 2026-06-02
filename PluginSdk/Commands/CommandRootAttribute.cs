using System;

namespace PluginSdk.Commands
{
    /// <summary>
    /// Declares the chat-command namespace (root) that a
    /// <see cref="CommandModule"/> contributes to. Every command in the module
    /// is reached as <c>!{prefix} {command path}</c>, e.g. a module with
    /// <c>[CommandRoot("ess")]</c> containing <c>[Command("save")]</c> exposes
    /// <c>!ess save</c>.
    ///
    /// <para>
    /// The bare <c>!{prefix}</c> shows an overview of the root's commands and
    /// <c>!{prefix} help [command]</c> shows usage, both filtered by the
    /// caller's permission level. These are generated automatically and need
    /// no handler.
    /// </para>
    /// <para>
    /// Several modules may share one prefix as long as they belong to the same
    /// plugin; their commands are merged under that root. Two different plugins
    /// declaring the same prefix is a conflict and the second registration is
    /// rejected.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandRootAttribute : Attribute
    {
        /// <summary>
        /// Short namespace token typed after the <c>!</c> prefix (e.g.
        /// <c>"ess"</c>). Matched case-insensitively; must be a single word
        /// with no whitespace.
        /// </summary>
        public string Prefix { get; }

        /// <summary>Human-readable title shown in the overview and used as the
        /// default chat author for replies. Defaults to <see cref="Prefix"/>.</summary>
        public string Title { get; }

        /// <summary>Optional one-line description shown in the overview header.</summary>
        public string Description { get; }

        public CommandRootAttribute(string prefix, string title = null, string description = null)
        {
            Prefix = prefix;
            Title = title;
            Description = description;
        }
    }
}
