﻿using Pandaros.WoWParser.Parser.FightMonitor;
using Pandaros.WoWParser.Parser.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Pandaros.WoWParser.Parser.Calculators
{
    public class AvoidanceCalculator : BaseCalculator
    {
        Dictionary<string, Dictionary<MissType, long>> _damageAvoidedByEntityfromType = new Dictionary<string, Dictionary<MissType, long>>();
        Dictionary<string, Dictionary<MissType, Dictionary<string, long>>> _damageAvoidedByEntityfromTypeByattack = new Dictionary<string, Dictionary<MissType, Dictionary<string, long>>>();
        Dictionary<string, long> _attacks = new Dictionary<string, long>();
        Dictionary<string, Dictionary<string, Dictionary<MissType, long>>> _attackAvoidedByEntityFromEntity = new Dictionary<string, Dictionary<string, Dictionary<MissType, long>>>(); 

        public AvoidanceCalculator(IPandaLogger logger, IStatsLogger reporter, ICombatState state, MonitoredFight fight) : base(logger, reporter, state, fight)
        {
            ApplicableEvents = new List<string>()
                {
                    LogEvents.SWING_MISSED,
                    LogEvents.SWING_DAMAGE,
                    LogEvents.RANGE_DAMAGE,
                    LogEvents.RANGE_MISSED,
                    LogEvents.SPELL_MISSED
                };
        }


        public override void CalculateEvent(ICombatEvent combatEvent)
        {
            if (combatEvent.DestFlags.FlagType == UnitFlags.UnitFlagType.Player)
            {
                _attacks.AddValue(combatEvent.DestName, 1);

                switch (combatEvent.EventName)
                {
                    case LogEvents.SWING_MISSED:
                    case LogEvents.RANGE_MISSED:
                    case LogEvents.SPELL_MISSED:
                        var spellMissed = (IMissed)combatEvent;

                        if (spellMissed.MissType != MissType.ABSORB && 
                            spellMissed.MissType != MissType.REFLECT && 
                            spellMissed.MissType != MissType.IMMUNE &&
                            spellMissed.MissType != MissType.EVADE)
                        {
                            _damageAvoidedByEntityfromType.AddValue(combatEvent.DestName, spellMissed.MissType, 1);
                            _attackAvoidedByEntityFromEntity.AddValue(combatEvent.DestName, combatEvent.SourceName, spellMissed.MissType, 1);

                            if (combatEvent is SwingBase swing)
                                _damageAvoidedByEntityfromTypeByattack.AddValue(combatEvent.DestName, spellMissed.MissType, "swing", 1);
                            else if (combatEvent is SpellBase spell)
                                _damageAvoidedByEntityfromTypeByattack.AddValue(combatEvent.DestName, spellMissed.MissType, spell.SpellName, 1);
                        }
                        break;
                }

            }
        }

        public override void FinalizeFight(ICombatEvent combatEvent)
        {
            List<List<string>> table = new List<List<string>>();
            var enums = Enum.GetValues(typeof(MissType)).Cast<MissType>().ToList();
            enums.Remove(MissType.ABSORB);
            enums.Remove(MissType.REFLECT);
            enums.Remove(MissType.IMMUNE);
            enums.Remove(MissType.EVADE);
            var all = enums.Select(e => e.ToString()).ToList();
            var total = _attacks.Sum(kvp => kvp.Value);
            all.Insert(0, $"Overall");
            all.Insert(0, $"attacksVsPlayer");
            table.Add(all);
            Dictionary<string, long> totals = new Dictionary<string, long>();

            foreach (var baseKvp in _damageAvoidedByEntityfromType)
            {
                var subTotal = baseKvp.Value.Sum(kvp => kvp.Value);
                total += subTotal;
                totals[baseKvp.Key] = subTotal;
            }
            
            foreach (var baseKvp in totals.OrderBy(i => i.Value).Reverse())
            {
                var row = new List<string>();
                if (_attacks.TryGetValue(baseKvp.Key, out var attacksVsPlayer))
                {
                    row.Add($"{baseKvp.Key} ({attacksVsPlayer})");
                    row.Add($"{baseKvp.Value} ({(Math.Round((double)baseKvp.Value / attacksVsPlayer, 2) * 100).ToString().PadRight(3).Substring(0, 3) }%)");
                    foreach (var missType in enums)
                    {
                        if (_damageAvoidedByEntityfromType[baseKvp.Key].TryGetValue(missType, out var missCount))
                            row.Add($"{missCount} ({(Math.Round((double)missCount / attacksVsPlayer, 2) * 100).ToString().PadRight(3).Substring(0, 3)}%)");
                        else
                            row.Add("0");
                    }
                }
                else
                {
                    row.Add(baseKvp.Key);
                    row.Add("0");
                    foreach (var missType in enums)
                        row.Add("0");
                }

                table.Add(row);
            }

            var length = new List<int>()
            {
                35
            };

            _statsReporting.ReportTable(table, "Avoidance", Fight, State, length);

            _statsReporting.Report(_attackAvoidedByEntityFromEntity, "Avoidance by Monster by Miss Type (% of all avoidance during fight)", Fight, State);
            _statsReporting.Report(_damageAvoidedByEntityfromTypeByattack, "Avoidance attack types", Fight, State);
        }

        public override void StartFight(ICombatEvent combatEvent)
        {
            
        }
    }
}
