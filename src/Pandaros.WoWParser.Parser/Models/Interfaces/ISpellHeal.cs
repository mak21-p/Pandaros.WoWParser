﻿namespace Pandaros.WoWParser.Parser.Models
{
    public interface ISpellHeal
    {
        int Absorbed { get; set; }
        bool Critical { get; set; }
        int HealAmount { get; set; }
        int Overhealing { get; set; }
    }
}