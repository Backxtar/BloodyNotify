using HarmonyLib;
using ProjectM;
using BloodyNotify.Structs;
using Bloodstone.API;
using System.Collections.Generic;
using BloodyNotify.Utils;
using System;
using System.Text;

namespace BloodyNotify.Hooks;

[HarmonyPatch]
internal class VBlood_Hook
{
    [HarmonyPatch(typeof(VBloodSystem), nameof(VBloodSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void OnUpdate(VBloodSystem __instance)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.VBLOOD) || __instance.EventList.IsEmpty) return;

        foreach (var _event in __instance.EventList)
        {
            if (!VWorld.Server.EntityManager.TryGetComponentData<PlayerCharacter>(_event.Target, out var _data)) continue;
            var _vblood = __instance._PrefabCollectionSystem.PrefabDataLookup[_event.Source].AssetName;
            Player _player = new(_data.UserEntity);
            bool _exist = false;

            foreach (var _kill in ActionScheduler.ACTIONS)
            {
                if (_kill.VBlood == _vblood.ToString())
                {
                    _exist = true;
                    break;
                }
            }

            if (!_exist)
            {
                KillEvent _kill = new(DateTime.Now, _vblood.ToString());
                ActionScheduler.ACTIONS.Add(_kill);
            }

            for (int i = 0; i < ActionScheduler.ACTIONS.Count; i++)
            {
                if (ActionScheduler.ACTIONS[i].VBlood == _vblood.ToString() &&
                    !ActionScheduler.ACTIONS[i].Killers.Contains(_player.Name))
                {
                    ActionScheduler.ACTIONS[i].Killers.Add(_player.Name);
                }
            }
        }
    }

    public static string GetAnnouncement(string vblood, List<string> killers)
    {
        var _vbloodLabel = Settings.GetVBlood(vblood);

        var _names = new StringBuilder();

        if (killers.Count == 0) return "";
        for (int i = 0; i < killers.Count; i++)
        {
            _names.Append(killers[i]);
            if (i < killers.Count - 2)
            {
                _names.Append($"{Settings.GetMessageTemplate("comma")} ");
                continue;
            }
            if (i < killers.Count - 1)
            {
                _names.Append($" {Settings.GetMessageTemplate("append")} ");
            }
        }

        return Settings.GetMessageTemplate("VBlood")
            .Replace("{user}", _names.ToString())
            .Replace("{vblood}", _vbloodLabel);
    }
}
