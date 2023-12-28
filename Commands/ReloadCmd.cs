using VampireCommandFramework;

namespace BloodyNotify.Commands;

[CommandGroup("notify")]
internal class ReloadCmd
{
    [Command("reload", "rl", description: "Reloads the config of BloodyNotify.", adminOnly: true)]
    public static void ReloadNotify(ChatCommandContext ctx)
    {
        Plugin.Settings.InitConfig();
        ctx.Reply("BloodyNotify config reloaded!");
    }
}
