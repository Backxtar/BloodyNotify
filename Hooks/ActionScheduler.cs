using Bloodstone.API;
using BloodyNotify.Structs;
using ProjectM;
using System;
using System.Collections.Generic;

namespace BloodyNotify.Hooks;

internal class ActionScheduler
{
    public static readonly List<KillEvent> ACTIONS = new();

    public static void HandleNotifyFrame()
    {
        for (int i = ACTIONS.Count - 1; i >= 0; i--)
        {
            if (DateTime.Now - ACTIONS[i].Executed > TimeSpan.FromSeconds(1))
            {
                var _message = VBlood_Hook.GetAnnouncement(ACTIONS[i].VBlood, ACTIONS[i].Killers);
                if (_message == null || _message == "") continue;
                ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, _message);
                ACTIONS.RemoveAt(i);
            }
        }
    }
}
