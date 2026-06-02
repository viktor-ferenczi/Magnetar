using System;
using VRage.Game.ModAPI;

namespace PluginSdk.Commands
{
    /// <summary>
    /// Sets the minimum <see cref="MyPromoteLevel"/> a caller must hold to run
    /// the decorated command. When absent a command defaults to
    /// <see cref="MyPromoteLevel.Admin"/>, so forgetting the attribute fails
    /// safe rather than exposing a command to everyone.
    ///
    /// <para>
    /// Commands the caller may not run are hidden from overview and help
    /// listings, so the attribute also controls discoverability.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class PermissionAttribute : Attribute
    {
        /// <summary>Minimum promote level required to invoke the command.</summary>
        public MyPromoteLevel Level { get; }

        public PermissionAttribute(MyPromoteLevel level)
        {
            Level = level;
        }
    }
}
