using System;
using System.Collections.Generic;

namespace BloodyNotify.Structs;

public struct KillEvent
{
    public DateTime Executed {  get; set; }
    public string VBlood { get; set; }
    public List<string> Killers { get; set; } = new();

    public KillEvent(DateTime _executed, string _vblood)
    {
        Executed = _executed;
        VBlood = _vblood;
    }
}
