﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class SpellEnergize : SpellBase, ISpellEnergize
    {
        public int EneryAmount { get; set; }
        public PowerType PowerType { get; set; }
    }
}
