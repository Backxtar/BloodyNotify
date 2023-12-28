using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Unity.Entities;

namespace BloodyNotify.Structs;

public readonly struct Settings
{
    private readonly ConfigFile CONFIG;
    private readonly ConfigEntry<bool> ENABLE_MOD;
    private readonly ConfigEntry<bool> PVP_KILLS;
    private readonly ConfigEntry<bool> NEW_PLAYER;
    private readonly ConfigEntry<bool> ADMIN_ONLINE;
    private readonly ConfigEntry<bool> VBLOOD_KILLS;

    private static readonly string CONFIG_PATH = Path.Combine(BepInEx.Paths.ConfigPath, "BloodyNotify");
    private static readonly string MESSAGES_PATH = Path.Combine(CONFIG_PATH, "messages.json");
    private static readonly string VBLOOD_PATH = Path.Combine(CONFIG_PATH, "vbloods.json");
    private static readonly string ADMIN_PATH = Path.Combine(CONFIG_PATH, "admins.json");

    public Settings(ConfigFile config)
    {
        CONFIG = config;
        ENABLE_MOD = CONFIG.Bind("Config", "EnableMod", true, "Enable or disable notifications");
        PVP_KILLS = CONFIG.Bind("Config", "PVPKills", true, "Enable pvp kill notifications");
        NEW_PLAYER = CONFIG.Bind("Config", "NewPlayer", true, "Enable new player notifications");
        ADMIN_ONLINE = CONFIG.Bind("Config", "AdminOnline", true, "Enable admin notifications");
        VBLOOD_KILLS = CONFIG.Bind("Config", "VBloodKills", true, "Enable vblood kill notifications");
    }

    public readonly void InitConfig()
    {
        string _json;
        Dictionary<string, string> _dic;

        WriteConfig(MESSAGES_PATH, ANNOUNCEMENTS);
        WriteConfig(VBLOOD_PATH, VBLOODS);
        WriteConfig(ADMIN_PATH, STAFF);

        ANNOUNCEMENTS.Clear();
        VBLOODS.Clear();
        STAFF.Clear();

        _json = File.ReadAllText(MESSAGES_PATH);
        _dic = JsonSerializer.Deserialize<Dictionary<string, string>>(_json);

        foreach ( var kvp in _dic )
        {
            ANNOUNCEMENTS.Add(kvp.Key, kvp.Value);
        }

        _json = File.ReadAllText(VBLOOD_PATH);
        _dic = JsonSerializer.Deserialize<Dictionary<string, string>>(_json);

        foreach (var kvp in _dic)
        {
            VBLOODS.Add(kvp.Key, kvp.Value);
        }

        _json = File.ReadAllText(ADMIN_PATH);
        _dic = JsonSerializer.Deserialize<Dictionary<string, string>>(_json);

        foreach (var kvp in _dic)
        {
            STAFF.Add(kvp.Key, kvp.Value);
        }

        Plugin.LogInstance.LogInfo($"Mod enabled: {ENABLE_MOD.Value}");
        Plugin.LogInstance.LogInfo($"PVP kills enabled: {PVP_KILLS.Value}");
        Plugin.LogInstance.LogInfo($"New players enabled: {NEW_PLAYER.Value}");
        Plugin.LogInstance.LogInfo($"Admins enabled: {ADMIN_ONLINE.Value}");
        Plugin.LogInstance.LogInfo($"VBloods enabled: {VBLOOD_KILLS.Value}");
    }

    public static void WriteConfig(string path, Dictionary<string, string> dic)
    {
        if (!Directory.Exists(CONFIG_PATH)) Directory.CreateDirectory(CONFIG_PATH);
        if (!File.Exists(path))
        {
             var _json = JsonSerializer.Serialize(dic, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, _json);
        }
    }

    public bool GetActiveSystem(Systems type)
    {
        return type switch
        {
            Systems.ENABLE => ENABLE_MOD.Value,
            Systems.VBLOOD => VBLOOD_KILLS.Value,
            Systems.PVP => PVP_KILLS.Value,
            Systems.PLAYERS => NEW_PLAYER.Value,
            Systems.ADMIN => ADMIN_ONLINE.Value,
            _ => false,
        };
    }

    public static string GetVBlood(string key)
    {
        return (key == null || !VBLOODS.ContainsKey(key)) ? VBLOODS["NoPrefabName"] : VBLOODS[key];
    }

    public static string GetMessageTemplate(string key)
    {
        return (key == null || !ANNOUNCEMENTS.ContainsKey(key)) ? key : ANNOUNCEMENTS[key];
    }

    public static string GetStaff(Entity user)
    {
        Player _player = new(user);
        return (_player == null || !STAFF.ContainsKey(_player.SteamID.ToString()) ? null : STAFF[_player.SteamID.ToString()]);
    }

    public static Dictionary<string, string> GetStaff()
    {
        return STAFF;
    }

    private static readonly Dictionary<string, string> ANNOUNCEMENTS = new()
    {
        { "NewUser" , "A new vampire has reached Bloody Mary!" },
        { "VBlood" , "{vblood} was killed by {user}." },
        { "PlayerKilled" , "{target} was killed by {killer}!" },
        { "StaffOnline", "{role} {user} is now online!" },
        { "StaffOffline", "{role} {user} is now offline!" },
        { "append", "and" },
        { "comma", "," }
    };

    private static readonly Dictionary<string, string> STAFF = new()
    {  
        { "123", "Role" },
        { "1234", "Role" }
    };

    private static readonly Dictionary<string, string> VBLOODS = new ()
    {
        { "CHAR_Wildlife_Wolf_VBlood", "Alpha Wolf" },
        { "CHAR_Bandit_Deadeye_Frostarrow_VBlood", "Keely the Frost Archer" },
        { "CHAR_Bandit_Foreman_VBlood", "Rufus the Foreman" },
        { "CHAR_Bandit_StoneBreaker_VBlood", "Errol the Stonebreaker" },
        { "CHAR_Bandit_Deadeye_Chaosarrow_VBlood", "Lidia the Chaos Archer" },
        { "CHAR_Undead_BishopOfDeath_VBlood", "Goreswine the Ravager" },
        { "CHAR_Bandit_Stalker_VBlood", "Grayson the Armourer" },
        { "CHAR_Vermin_DireRat_VBlood", "Putrid Rat" },
        { "CHAR_Bandit_Bomber_VBlood", "Clive the Firestarter" },
        { "CHAR_Wildlife_Poloma_VBlood", "Polora the Feywalker" },
        { "CHAR_Wildlife_Bear_Dire_Vblood", "Ferocious Bear" },
        { "CHAR_Undead_Priest_VBlood", "Nicholaus the Fallen" },
        { "CHAR_Bandit_Tourok_VBlood", "Quincey the Bandit King" },
        { "CHAR_Villager_Tailor_VBlood", "Beatrice the Tailor" },
        { "CHAR_Militia_Guard_VBlood", "Vincent the Frostbringer" },
        { "CHAR_Farmlands_Nun_VBlood", "Christina the Sun Priestess" },
        { "CHAR_VHunter_Leader_VBlood", "Tristan the Vampire Hunter" },
        { "CHAR_Undead_BishopOfShadows_VBlood", "Leandra the Shadow Priestess" },
        { "CHAR_Geomancer_Human_VBlood", "Terah the Geomancer" },
        { "CHAR_Militia_Longbowman_LightArrow_Vblood", "Meredith the Bright Archer" },
        { "CHAR_Wendigo_VBlood", "Frostmaw the Mountain Terror" },
        { "CHAR_Militia_Leader_VBlood", "Octavian the Militia Captain" },
        { "CHAR_Militia_BishopOfDunley_VBlood", "Raziel the Shepherd" },
        { "CHAR_Spider_Queen_VBlood", "Ungora the Spider Queen" },
        { "CHAR_Cursed_ToadKing_VBlood", "The Duke of Balaton" },
        { "CHAR_VHunter_Jade_VBlood", "Jade the Vampire Hunter" },
        { "CHAR_Undead_ZealousCultist_VBlood", "Foulrot the Soultaker" },
        { "CHAR_WerewolfChieftain_VBlood", "Willfred the Werewolf Chief" },
        { "CHAR_ArchMage_VBlood", "Mairwyn the Elementalist" },
        { "CHAR_Town_Cardinal_VBlood", "Azariel the Sunbringer" },
        { "CHAR_Winter_Yeti_VBlood", "Terrorclaw the Ogre" },
        { "CHAR_Harpy_Matriarch_VBlood", "Morian the Stormwing Matriarch" },
        { "CHAR_Cursed_Witch_VBlood", "Matka the Curse Weaver" },
        { "CHAR_BatVampire_VBlood", "Nightmarshal Styx the Sunderer" },
        { "CHAR_Cursed_MountainBeast_VBlood", "Gorecrusher the Behemoth" },
        { "CHAR_Manticore_VBlood", "The Winged Horror" },
        { "CHAR_Paladin_VBlood", "Solarus the Immaculate" },
        { "CHAR_Bandit_GraveDigger_VBlood_UNUSED", "CHAR_Bandit_GraveDigger_VBlood_UNUSED" },
        { "CHAR_Bandit_Leader_VBlood_UNUSED", "CHAR_Bandit_Leader_VBlood_UNUSED" },
        { "CHAR_Bandit_Miner_VBlood_UNUSED", "CHAR_Bandit_Miner_VBlood_UNUSED" },
        { "CHAR_Bandit_Thief_VBlood_UNUSED", "CHAR_Bandit_Thief_VBlood_UNUSED" },
        { "CHAR_ChurchOfLight_Cardinal_VBlood", "Azariel the Sunbringer" },
        { "CHAR_ChurchOfLight_Overseer_VBlood", "Sir Magnus the Overseer" },
        { "CHAR_ChurchOfLight_Paladin_VBlood", "Solarus the Immaculate" },
        { "CHAR_ChurchOfLight_Sommelier_VBlood", "Baron du Bouchon the Sommelier" },
        { "CHAR_Forest_Bear_Dire_Vblood", "Ferocious Bear" },
        { "CHAR_Forest_Wolf_VBlood", "Alpha Wolf" },
        { "CHAR_Geomancer_Golem_VBlood", "CHAR_Geomancer_Golem_VBlood" },
        { "CHAR_Gloomrot_Iva_VBlood", "Ziva the Engineer" },
        { "CHAR_Gloomrot_Monster_VBlood", "Adam the Firstborn" },
        { "CHAR_Gloomrot_Purifier_VBlood", "Angram the Purifier" },
        { "CHAR_Gloomrot_RailgunSergeant_VBlood", "Voltatia the Power Master" },
        { "CHAR_Gloomrot_TheProfessor_VBlood", "Henry Blackbrew the Doctor" },
        { "CHAR_Gloomrot_Voltage_VBlood", "Domina the Blade Dancer" },
        { "CHAR_Militia_Glassblower_VBlood", "Grethel the Glassblower" },
        { "CHAR_Militia_Hound_VBlood", "CHAR_Militia_Hound_VBlood" },
        { "CHAR_Militia_HoundMaster_VBlood", "CHAR_Militia_HoundMaster_VBlood" },
        { "CHAR_Militia_Nun_VBlood", "Christina the Sun Priestess" },
        { "CHAR_Militia_Scribe_VBlood", "Maja the Dark Savant" },
        { "CHAR_Poloma_VBlood", "Polora the Feywalker" },
        { "CHAR_Undead_CursedSmith_VBlood", "Cyril the Cursed Smith" },
        { "CHAR_Undead_Infiltrator_VBlood", "Bane the Shadowblade" },
        { "CHAR_Undead_Leader_Vblood", "Kriig the Undead General" },
        { "CHAR_Villager_CursedWanderer_VBlood", "The Old Wanderer" },
        { "NoPrefabName", "VBlood Boss" }
    };
}
