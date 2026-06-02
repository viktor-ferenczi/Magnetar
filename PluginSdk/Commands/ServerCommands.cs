using System;
using System.Reflection;

namespace PluginSdk.Commands
{
    /// <summary>
    /// The entry point a plugin calls to register chat commands, analogous to
    /// calling <c>Harmony.PatchAll()</c>. Registration is explicit so the host
    /// never silently discovers command classes: the plugin either hands over
    /// its assembly to be scanned, or names the module types directly (less
    /// magic, fewer surprises).
    ///
    /// <para>
    /// The plugin always passes its own assembly, which is used to attribute
    /// ownership of the registered command prefixes. Because ownership comes
    /// from the assembly rather than from a host-managed time window, a plugin
    /// may register from its <c>Init()</c> or at any later point.
    /// </para>
    /// </summary>
    public static class ServerCommands
    {
        /// <summary>
        /// The host-installed registrar. The host sets this once at startup; it
        /// is not for plugins to assign.
        /// </summary>
        public static ICommandRegistrar Registrar { get; set; }

        /// <summary>
        /// Registers every command module in <paramref name="assembly"/> — the
        /// usual call is <c>ServerCommands.Register(Assembly.GetExecutingAssembly())</c>.
        /// </summary>
        public static void Register(Assembly assembly)
            => Require().Register(assembly);

        /// <summary>
        /// Registers the named <see cref="CommandModule"/> types explicitly,
        /// e.g. <c>ServerCommands.Register(Assembly.GetExecutingAssembly(), typeof(AdminCommands), typeof(InfoCommands))</c>.
        /// Prefer this over the assembly scan when you want the set of command
        /// classes to be obvious and compiler-checked.
        /// </summary>
        public static void Register(Assembly assembly, params Type[] moduleTypes)
            => Require().Register(assembly, moduleTypes);

        private static ICommandRegistrar Require()
            => Registrar ?? throw new InvalidOperationException(
                "No command registrar is available. ServerCommands.Register requires a host " +
                "that supports chat commands.");
    }
}
