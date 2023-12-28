using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using BepInEx;
using Bloodstone.API;
using VampireCommandFramework;
using BloodyNotify.Utils;
using BloodyNotify.Structs;
using BloodyNotify.Hooks;

namespace BloodyNotify;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Reloadable]
public class Plugin : BasePlugin, IRunOnInitialized
{
    private Harmony _harmony;

    public static ManualLogSource LogInstance { get; private set; }
    public static Settings Settings { get; private set; }

    public override void Load()
    {
        LogInstance = Log;
        Settings = new (Config);
        Settings.InitConfig();

        if (!VWorld.IsServer) Log.LogWarning("This plugin is a server-only plugin.");
        CommandRegistry.RegisterAll();
        Bloodstone.Hooks.GameFrame.OnUpdate += ActionScheduler.HandleNotifyFrame;
    }

    public void OnGameInitialized()
    {
        if (VWorld.IsClient) return;
        
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Core.InitializeAfterLoaded();
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        Config.Clear();
        _harmony.UnpatchSelf();
        Bloodstone.Hooks.GameFrame.OnUpdate -= ActionScheduler.HandleNotifyFrame;
        return true;
    }
}