﻿using BepInEx.Logging;
using System.Runtime.CompilerServices;
using Unity.Entities;

namespace BloodyNotify.Utils;

internal static class Core
{
    public static World Server { get; } = GetWorld("Server") ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
    public static EntityManager EntityManager { get; } = Server.EntityManager;
    public static ManualLogSource Log = Plugin.LogInstance;
    public static PlayerService Players { get; internal set; }
    public static UnitSpawnerService UnitSpawner { get; internal set; }
    public static PrefabService Prefabs { get; internal set; }


    // Check paramater
    private static bool _hasInitialized = false;

    public static void LogException(System.Exception e, [CallerMemberName] string caller = null)
    {
        Core.Log.LogError($"Failure in {caller}\nMessage: {e.Message} Inner:{e.InnerException?.Message}\n\nStack: {e.StackTrace}\nInner Stack: {e.InnerException?.StackTrace}");
    }

    internal static void InitializeAfterLoaded()
    {
        if (_hasInitialized) return;

        // TODO: probably changing when I refactor further.
        Players = new();
        UnitSpawner = new();
        Prefabs = new();
        _hasInitialized = true;
        Log.LogInfo($"{nameof(InitializeAfterLoaded)} completed");
    }

    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }

        return null;
    }
}