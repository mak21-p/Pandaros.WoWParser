﻿namespace Pandaros.WoWParser.Parser.Models
{
    public interface IMissed
    {
        MissType MissType { get; set; }
        int Absorbed { get; set; }
    }
}