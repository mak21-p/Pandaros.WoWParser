﻿using Pandaros.WoWParser.Parser.FightMonitor;
using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pandaros.WoWParser.Parser.Calculators
{
    public class DispelCalculator : BaseCalculator
    {
        // Player, Dispell Spell, Dispelled, count
        Dictionary<string, Dictionary<string, Dictionary<string, long>>> _Dispells = new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();

        public DispelCalculator(IPandaLogger logger, IStatsLogger reporter, ICombatState state, MonitoredFight fight) : base(logger, reporter, state, fight)
        {
            ApplicableEvents = new List<string>()
            {
                LogEvents.SPELL_DISPEL
            };
        }

        public override void CalculateEvent(ICombatEvent combatEvent)
        {
            if (combatEvent.SourceFlags.FlagType != UnitFlags.UnitFlagType.Player && combatEvent.SourceFlags.Controller != UnitFlags.UnitController.Player)
                return;
            var spell = (SpellDispel)combatEvent;

            if (State.TryGetSourceOwnerName(combatEvent, out string owner))
                _Dispells.AddValue(owner, spell.SpellName, spell.ExtraSpellName, 1);
            else
                _Dispells.AddValue(combatEvent.SourceName, spell.SpellName, spell.ExtraSpellName, 1);
        }

        public override void FinalizeFight(ICombatEvent combatEvent)
        {
            _statsReporting.Report(_Dispells, "Dispells", Fight, State);
        }

        public override void StartFight(ICombatEvent combatEvent)
        {
            
        }
    }
}
