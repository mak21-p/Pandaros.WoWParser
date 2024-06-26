﻿using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Parsers
{
    public class SpellFailedParser : SpellParser, ICombatParser<SpellFailed>
    {
        public new SpellFailed Parse(DateTime timestamp, string eventName, string[] eventData)
        {
            return Parse(timestamp, eventName, eventData, new SpellFailed());
        }

        public SpellFailed Parse(DateTime timestamp, string eventName, string[] eventData, SpellFailed obj)
        {
            obj = (SpellFailed)base.Parse(timestamp, eventName, eventData, obj);
            obj.FailedType = eventData[Indexes.SPELL_CAST_FAILED.CastFailedReason];
            return obj;
        }
    }
}
