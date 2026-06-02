namespace PluginSdk.Commands
{
    /// <summary>
    /// Base class for a group of chat commands. Decorate the subclass with
    /// <see cref="CommandRootAttribute"/> to choose its <c>!prefix</c>, and
    /// each handler method with <see cref="CommandAttribute"/>.
    ///
    /// <para>
    /// A new instance is created for every command invocation and
    /// <see cref="Context"/> is assigned before the handler runs, so modules
    /// should be stateless; keep persistent state in the owning plugin and
    /// reach it via static members or fields the plugin sets up.
    /// </para>
    /// </summary>
    public abstract class CommandModule
    {
        /// <summary>The environment for the current invocation: caller,
        /// arguments and reply helpers.</summary>
        public CommandContext Context { get; internal set; }
    }
}
