using Bloodstone.API;
using BloodyNotify.Structs;
using HarmonyLib;
using ProjectM;
using Unity.Collections;

namespace BloodyNotify.Hooks;

[HarmonyPatch]
internal class PlayerKilled_Hook
{
    [HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
    [HarmonyPostfix]
    public static void OnUpdate(DeathEventListenerSystem __instance)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.PVP)) return;
        NativeArray<DeathEvent> _events = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
        var _manager = VWorld.Server.EntityManager;

        foreach (var _event in _events)
        {
            if (!_manager.TryGetComponentData<PlayerCharacter>(_event.Died, out var _defeat)) continue;
            if (!_manager.TryGetComponentData<PlayerCharacter>(_event.Killer, out var _victory)) continue;
            Player _killed = new(_defeat.UserEntity), _killer = new(_victory.UserEntity);

            if (_killed.SteamID == _killer.SteamID) continue;
            var _message = Settings.GetMessageTemplate("PlayerKilled")
                .Replace("{killer}", _killer.Name)
                .Replace("{target}", _killed.Name);
            ServerChatUtils.SendSystemMessageToAllClients(_manager, _message);
        }
    }
}
