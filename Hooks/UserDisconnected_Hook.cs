using Bloodstone.API;
using BloodyNotify.Structs;
using HarmonyLib;
using ProjectM;
using Stunlock.Network;

namespace BloodyNotify.Hooks;

[HarmonyPatch]
internal class UserDisconnected_Hook
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
    [HarmonyPrefix]
    public static void OnUpdate(ServerBootstrapSystem __instance, NetConnectionId netConnectionId, ConnectionStatusChangeReason connectionStatusReason)
    {
        if (!Plugin.Settings.GetActiveSystem(Systems.ADMIN)) return;
        if (connectionStatusReason == ConnectionStatusChangeReason.IncorrectPassword) return;

        var _userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var _serverClient = __instance._ApprovedUsersLookup[_userIndex];
        var _user = _serverClient.UserEntity;
        Player _player = new(_user);
        var _role = Settings.GetStaff(_user);

        if (_role == null || _role == "") return;
        var _message = Settings.GetMessageTemplate("StaffOffline")
            .Replace("{role}", _role)
            .Replace("{user}", _player.Name);
        ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, _message);
    }
}
