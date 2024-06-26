﻿using Pandaros.WoWParser.Parser.FightMonitor;
using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pandaros.WoWParser.Parser.Calculators
{
    public class SpellsCastByNPCCalculator : BaseCalculator
    {
        Dictionary<string, Dictionary<SpellSchool, Dictionary<string, List<long>>>> _spellsCast = new Dictionary<string, Dictionary<SpellSchool, Dictionary<string, List<long>>>>();

        public SpellsCastByNPCCalculator(IPandaLogger logger, IStatsLogger reporter, ICombatState state, MonitoredFight fight) : base(logger, reporter, state, fight)
        {
            ApplicableEvents = new List<string>()
            {
                LogEvents.SPELL_DAMAGE,
                LogEvents.RANGE_DAMAGE,
                LogEvents.SWING_DAMAGE,
                LogEvents.SPELL_PERIODIC_DAMAGE,
                LogEvents.DAMAGE_SHIELD,
                LogEvents.SPELL_CAST_SUCCESS
            };
        }

        public override void CalculateEvent(ICombatEvent combatEvent)
        {
            if (combatEvent.SourceFlags.FlagType == UnitFlags.UnitFlagType.Player || combatEvent.SourceFlags.Controller == UnitFlags.UnitController.Player)
                return;

            if (combatEvent is ISpell spell)
            {
                if (combatEvent is IDamage damage)
                {
                    _spellsCast.AddValue(combatEvent.SourceName, spell.School, spell.SpellName, 0, 1);
                    _spellsCast.AddValue(combatEvent.SourceName, spell.School, spell.SpellName, 1, damage.Damage);
                }
                else
                {
                    if (_spellsCast.TryGetValue(combatEvent.SourceName, out var casts) && 
                        casts.TryGetValue(spell.School, out var spellNames) &&
                        spellNames.TryGetValue(spell.SpellName, out var castCounts) &&
                        castCounts.Count == 1)
                        _spellsCast.AddValue(combatEvent.SourceName, spell.School, spell.SpellName, 0, 1);
                    else 
                        _spellsCast.AddValue(combatEvent.SourceName, spell.School, spell.SpellName, 0, 1);
                }
            }
            else if (combatEvent is IDamage damage)
            {
                _spellsCast.AddValue(combatEvent.SourceName, SpellSchool.Physical, "Swing", 0, 1);
                _spellsCast.AddValue(combatEvent.SourceName, SpellSchool.Physical, "Swing", 1, damage.Damage);
            }
                
        }

        public override void FinalizeFight(ICombatEvent combatEvent)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>()
            {
                "NPC",
                "School",
                "Attack",
                "Count",
                "Damage"
            });

            var length = new List<int>()
            {
                45,
                13,
                30,
                13,
                13
            };

            foreach (var npc in _spellsCast)
            {
                table.Add(new List<string>()
                {
                    npc.Key,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty
                });
                foreach (var school in npc.Value)
                {
                    table.Add(new List<string>()
                    {
                        string.Empty,
                        school.Key.ToString(),
                        string.Empty,
                        string.Empty,
                        string.Empty
                    });

                    foreach (var attack in school.Value)
                    {
                        if (attack.Value.Count == 2)
                            table.Add(new List<string>()
                            {
                                string.Empty,
                                string.Empty,
                                attack.Key,
                                attack.Value[0].ToString("N"),
                                attack.Value[1].ToString("N")
                            });
                        else
                            table.Add(new List<string>()
                            {
                                string.Empty,
                                string.Empty,
                                attack.Key,
                                attack.Value[0].ToString("N"),
                                "N/A"
                            });
                    }
                }
            }

            _statsReporting.ReportTable(table, "Attacks by NPC", Fight, State, length);
        }

        public override void StartFight(ICombatEvent combatEvent)
        {
            
        }
    }
}
