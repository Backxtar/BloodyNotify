using ProjectM.Network;
using ProjectM;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using BloodyNotify.Structs;
using Bloodstone.API;

namespace BloodyNotify.Utils;

internal class PlayerService
{
    Dictionary<FixedString64, PlayerData> NamePlayerCache = new();
    Dictionary<ulong, PlayerData> SteamPlayerCache = new();

    internal bool TryFindSteam(ulong steamId, out PlayerData playerData)
    {
        return SteamPlayerCache.TryGetValue(steamId, out playerData);
    }

    internal bool TryFindName(FixedString64 name, out PlayerData playerData)
    {
        return NamePlayerCache.TryGetValue(name, out playerData);
    }

    internal PlayerService()
    {
        NamePlayerCache.Clear();
        SteamPlayerCache.Clear();
        EntityQuery _query = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[]
                {
                ComponentType.ReadOnly<User>()
                },
            Options = EntityQueryOptions.IncludeDisabled
        });

        var _userEntities = _query.ToEntityArray(Allocator.Temp);
        foreach (var _entity in _userEntities)
        {
            var _userData = Core.EntityManager.GetComponentData<User>(_entity);
            PlayerData playerData = new(_userData.CharacterName, _userData.PlatformId, _userData.IsConnected, _entity, _userData.LocalCharacter._Entity);

            NamePlayerCache.TryAdd(_userData.CharacterName.ToString().ToLower(), playerData);
            SteamPlayerCache.TryAdd(_userData.PlatformId, playerData);
        }


        var _onlinePlayers = NamePlayerCache.Values.Where(p => p.IsOnline).Select(p => $"\t{p.CharacterName}");
        Core.Log.LogWarning($"Player Cache Created with {NamePlayerCache.Count} entries total, listing {_onlinePlayers.Count()} online:");
        Core.Log.LogWarning(string.Join("\n", _onlinePlayers));
    }

    internal void UpdatePlayerCache(Entity userEntity, string oldName, string newName, bool forceOffline = false)
    {
        var _userData = Core.EntityManager.GetComponentData<User>(userEntity);
        NamePlayerCache.Remove(oldName.ToLower());
        if (forceOffline) _userData.IsConnected = false;
        PlayerData playerData = new(newName, _userData.PlatformId, _userData.IsConnected, userEntity, _userData.LocalCharacter._Entity);

        NamePlayerCache[newName.ToLower()] = playerData;
        SteamPlayerCache[_userData.PlatformId] = playerData;
    }

    internal bool RenamePlayer(Entity userEntity, Entity charEntity, FixedString64 newName)
    {
        var _des = Core.Server.GetExistingSystem<DebugEventsSystem>();
        var _networkId = Core.EntityManager.GetComponentData<NetworkId>(userEntity);
        var _userData = Core.EntityManager.GetComponentData<User>(userEntity);
        var _renameEvent = new RenameUserDebugEvent
        {
            NewName = newName,
            Target = _networkId
        };
        var fromCharacter = new FromCharacter
        {
            User = userEntity,
            Character = charEntity
        };

        _des.RenameUser(fromCharacter, _renameEvent);
        UpdatePlayerCache(userEntity, _userData.CharacterName.ToString(), newName.ToString());
        return true;
    }

    public static bool IsNewUser(Entity userEntity)
    {
        var userComponent = GetUserComponente(userEntity);
        return userComponent.CharacterName.IsEmpty;
    }

    public static User GetUserComponente(Entity userEntity)
    {
        return VWorld.Server.EntityManager.GetComponentData<User>(userEntity);
    }

    public static string GetCharacterName(Entity userEntity)
    {
        return GetUserComponente(userEntity).CharacterName.ToString();
    }

    public static string GetCharacterName(User user)
    {
        return user.CharacterName.ToString();
    }

    public static IEnumerable<Entity> GetUsersOnline()
    {

        NativeArray<Entity> _userEntities = VWorld.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
        int len = _userEntities.Length;
        for (int i = 0; i < len; ++i)
        {
            if (_userEntities[i].Read<User>().IsConnected)
                yield return _userEntities[i];
        }

    }
}
