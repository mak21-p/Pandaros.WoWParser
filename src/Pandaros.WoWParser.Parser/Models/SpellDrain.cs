﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class SpellDrain : SpellBase, ISpellDrain
    {
        public int DrainAmount { get; set; }
        public PowerType PowerType { get; set; }
        public int ExtraDrainAmount { get; set; }
    }
}
