﻿using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Parsers
{
    public class SpellEnergizeParser : SpellParser, ICombatParser<SpellEnergize>
    {
        public new SpellEnergize Parse(DateTime timestamp, string eventName, string[] eventData)
        {
            return Parse(timestamp, eventName, eventData, new SpellEnergize());
        }

        public SpellEnergize Parse(DateTime timestamp, string eventName, string[] eventData, SpellEnergize obj)
        {
            obj = (SpellEnergize)base.Parse(timestamp, eventName, eventData, obj);
            obj.EneryAmount = eventData[Indexes.SPELL_ENERGIZE.EneryAmount].ToInt();
            obj.PowerType = eventData[Indexes.SPELL_ENERGIZE.PowerType].ToPowerType();

            return obj;
        }
    }
}
