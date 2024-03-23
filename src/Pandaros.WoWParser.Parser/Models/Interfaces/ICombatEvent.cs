﻿using System;

namespace Pandaros.WoWParser.Parser.Models
{
    public interface ICombatEvent
    {
        UnitFlags DestFlags { get; set; }
        string DestGuid { get; set; }
        string DestName { get; set; }
        string EventName { get; set; }
        string[] EventParameters { get; set; }
        UnitFlags SourceFlags { get; set; }
        string SourceGuid { get; set; }
        string SourceName { get; set; }
        DateTime Timestamp { get; set; }
    }
}