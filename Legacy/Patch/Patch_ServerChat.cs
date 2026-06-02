using System;
using HarmonyLib;
using Pulsar.Legacy.Commands;
using Pulsar.Legacy.Loader;
using Pulsar.Shared;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Gui;
using VRage.Network;

namespace Pulsar.Legacy.Patch;

/// <summary>
/// Intercepts global chat on the server before it is relayed to other players.
/// When a message in the global channel starts with the command prefix and is
/// handled by a registered command root, the original handler is skipped so the
/// command text is never broadcast or written to the chat log.
/// </summary>
[HarmonyPatchCategory("Late")]
[HarmonyPatch(typeof(MyMultiplayerBase), "OnChatMessageReceived_Server")]
[HarmonyPatch([typeof(ChatMsg)])]
public static class Patch_ServerChat
{
    public static bool Prefix(ChatMsg msg)
    {
        try
        {
            if (msg.Channel != (byte)ChatChannel.Global)
                return true;

            string text = msg.Text;
            if (string.IsNullOrEmpty(text) || text[0] != '!')
                return true;

            CommandService service = PluginLoader.Instance?.Commands;
            if (service is null)
                return true;

            ulong sender = MyEventContext.Current.Sender.Value;
            if (sender == 0UL)
                return true;

            return !service.HandleChat(sender, text);
        }
        catch (Exception e)
        {
            LogFile.Error($"Error in server chat command interception: {e}");
            return true;
        }
    }
}
