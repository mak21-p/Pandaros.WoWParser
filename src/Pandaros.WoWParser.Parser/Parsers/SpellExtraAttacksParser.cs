﻿using Pandaros.WoWParser.Parser.Models;
using System;

namespace Pandaros.WoWParser.Parser.Parsers
{
    public class SpellExtraAttacksParser : SpellParser, ICombatParser<SpellExtraAttacks>
    {
        public new SpellExtraAttacks Parse(DateTime timestamp, string eventName, string[] eventData)
        {
            return Parse(timestamp, eventName, eventData, new SpellExtraAttacks());
        }

        public SpellExtraAttacks Parse(DateTime timestamp, string eventName, string[] eventData, SpellExtraAttacks obj)
        {
            obj = (SpellExtraAttacks)base.Parse(timestamp, eventName, eventData, obj);
            obj.Amount = eventData[Indexes.SPELL_EXTRA_ATTACKS.Amount].ToInt();

            return obj;
        }
    }
}
