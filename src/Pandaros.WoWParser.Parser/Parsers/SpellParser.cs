﻿using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Parsers
{
    public class SpellParser : BaseParser, ICombatParser<SpellBase>
    {
        public new virtual SpellBase Parse(DateTime timestamp, string eventName, string[] eventData)
        {
            return Parse(timestamp, eventName, eventData, new SpellBase());
        }

        public virtual SpellBase Parse(DateTime timestamp, string eventName, string[] eventData, SpellBase obj)
        {
            obj = (SpellBase)base.Parse(timestamp, eventName, eventData, obj);
            obj.SpellId = eventData[Indexes.SPELL_CAST_START.CastSpellId].ToInt();
            obj.SpellName = eventData[Indexes.SPELL_CAST_START.CastSpellName];
            obj.School = (SpellSchool)eventData[Indexes.SPELL_CAST_START.CastSpellSchool].ToInt();
            return obj;
        }
    }
}
