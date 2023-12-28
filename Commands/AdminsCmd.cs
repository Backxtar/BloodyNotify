using BloodyNotify.Structs;
using BloodyNotify.Utils;
using System.Collections.Generic;
using System.Text;
using VampireCommandFramework;

namespace BloodyNotify.Commands;

internal class AdminsCmd
{
    [Command("admins", "a", description: "Shows online Staff members.", adminOnly: false)]
    public static void WhoIsOnline(ChatCommandContext ctx)
    {
        var _users = PlayerService.GetUsersOnline();
        var _staff = Settings.GetStaff();

        StringBuilder _builder = new();
        foreach (var _user in _users)
        {
            Player _player = new(_user);
            foreach (KeyValuePair<string, string> _kvp in _staff)
            {
                if (_player.SteamID.ToString() == _kvp.Key)
                {
                    string _role = _kvp.Value.Replace("</color>", "");
                    _builder
                        .Append(_role)
                        .Append(_player.Name)
                        .Append("</color>");
                    _builder.Append(' ');
                }
            }
        }
        if (_builder.Length == 0)
        {
            ctx.Reply("There is no staff online.");
            return;
        }
        ctx.Reply($"Active staff: {_builder.ToString()}");
    }
}
