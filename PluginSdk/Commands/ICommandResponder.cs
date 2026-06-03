namespace PluginSdk.Commands
{
    /// <summary>
    /// Sink that delivers a <see cref="CommandReply"/> to chat. Implemented by
    /// the host (which routes to the game's chat send) and by tests (which
    /// capture replies). Keeping this an interface lets the whole command
    /// pipeline run without a live game session.
    /// </summary>
    public interface ICommandResponder
    {
        /// <summary>
        /// Delivers <paramref name="reply"/>. When
        /// <see cref="CommandReply.Broadcast"/> is false the message is sent
        /// only to <paramref name="caller"/>; otherwise to all players.
        /// </summary>
        void Send(in CommandReply reply, in CommandCaller caller);
    }
}
