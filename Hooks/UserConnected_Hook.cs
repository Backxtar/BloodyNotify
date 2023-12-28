using Bloodstone.API;
using BloodyNotify.Structs;
using BloodyNotify.Utils;
using HarmonyLib;
using ProjectM;
using Stunlock.Network;
using Unity.Entities;

namespace BloodyNotify.Hooks;

[HarmonyPatch]
internal class UserConnected_Hook
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    [HarmonyPrefix]
    public static void OnUpdate(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        var _userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var _serverClient = __instance._ApprovedUsersLookup[_userIndex];
        var _user = _serverClient.UserEntity;

        AnnounceNewUser(_user);
        AnnounceAdmin(_user);
    }

    private static void AnnounceNewUser(Entity user)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.PLAYERS)) return;
        if (!PlayerService.IsNewUser(user)) return;
        var _message = Settings.GetMessageTemplate("NewUser");
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, _message);
    }

    private static void AnnounceAdmin(Entity user)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.ADMIN)) return;
        Player _player = new(user);
        var _role = Settings.GetStaff(user);
        if (_role == null || _role == "") return;
        var _message = Settings.GetMessageTemplate("StaffOnline")
            .Replace("{role}", _role)
            .Replace("{user}", _player.Name);
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, _message);
    }
}
