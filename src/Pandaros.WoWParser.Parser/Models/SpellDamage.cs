﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Models
{
    public class SpellDamage : SpellBase, IDamage
    {
        public int Damage { get; set; }
        public int Overkill { get; set; }
        public SpellSchool DamageSchool { get; set; }
        public int Resisted { get; set; }
        public int Blocked { get; set; }
        public int Absorbed { get; set; }
        public bool Critical { get; set; }
        public bool Glancing { get; set; }
        public bool Crushing { get; set; }
    }
}
