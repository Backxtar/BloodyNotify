using Bloodstone.API;
using ProjectM;
using System.Collections.Generic;

namespace BloodyNotify.Utils;

internal class PrefabService
{
    internal Dictionary<string, (string Name, PrefabGUID Prefab)> NameToGuid { get; init; } = new();

    internal PrefabService()
    {
        var _collectionSystem = Core.Server.GetExistingSystem<PrefabCollectionSystem>();
        var _spawnable = _collectionSystem.SpawnableNameToPrefabGuidDictionary;

        Core.Log.LogDebug($"Spawnable prefabs: {_spawnable.Count}");
        foreach (var _kvp in _spawnable)
        {
            bool _success = NameToGuid.TryAdd(_kvp.Key.ToLowerInvariant(), (_kvp.Key, _kvp.Value));
            if (!_success) Core.Log.LogDebug($"{_kvp.Key} exist already, skipping.");
        }
    }

    internal bool TryGetItem(string input, out PrefabGUID prefab)
    {
        var _lower = input.ToLowerInvariant();
        var _output = NameToGuid.TryGetValue(_lower, out var guidRec) || NameToGuid.TryGetValue($"item_{_lower}", out guidRec);
        prefab = guidRec.Prefab;
        return _output;
    }

    internal PrefabGUID GetGUIDByName(string value)
    {
        return NameToGuid[value.ToLowerInvariant()].Prefab;
    }

    public static string GetPrefabName(PrefabGUID hashCode)
    {
        var s = VWorld.Server.GetExistingSystem<PrefabCollectionSystem>();
        string name = "Nonexistent";
        if (hashCode.GuidHash == 0)
        {
            return name;
        }
        try
        {
            name = s.PrefabLookupMap[hashCode].ToString();
        }
        catch
        {
            name = "NoPrefabName";
        }
        return name;
    }
}
