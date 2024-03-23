using MongoDB.Driver;
using Pandaros.WoWParser.Parser.Calculators;
using Pandaros.WoWParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pandaros.WoWParser.Parser.FightMonitor
{
    public class FightMonitorFactory : IFightMonitorFactory
    {
        public static List<string> CombatEventsTriggerInFight { get; set; } = new List<string>()
        {
            LogEvents.SPELL_DAMAGE,
            LogEvents.SPELL_PERIODIC_DAMAGE,
            LogEvents.RANGE_MISSED,
            LogEvents.RANGE_DAMAGE,
            LogEvents.SPELL_MISSED,
            LogEvents.SWING_DAMAGE,
            LogEvents.SWING_MISSED,
            LogEvents.UNIT_DIED
        };

        IPandaLogger _logger;
        IStatsLogger _reporter;

        public FightMonitorFactory(IPandaLogger logger, IStatsLogger reporter)
        {
            _reporter = reporter;
            _logger = logger;
        }

        static string ExtractHexPortionWithRegex(string hexValue)
        {
            // Regex to find the sequence after "0xF13" and any number of zeros, capturing the hex sequence that follows
            var regex = new Regex("0xF130*([a-zA-Z0-9_]*)([[a-zA-Z0-9_]{6})");
            var match = regex.Match(hexValue); if (match.Success)
            {
                // Return the captured group which is the interesting portion
                var result =  Convert.ToInt32(match.Groups[1].Value, 16);
                return result.ToString();
            }
            return string.Empty;
        }

        public bool IsMonitoredFight(ICombatEvent evnt, ICombatState state)
        {
            if (!state.InFight && CombatEventsTriggerInFight.Contains(evnt.EventName))
            {
                string npcName = string.Empty;
                string npcId = string.Empty;

                if (evnt.SourceFlags.Controller == UnitFlags.UnitController.Npc)
                {
                    npcName = evnt.SourceName;
                    npcId = evnt.SourceGuid;
                }
                else if (evnt.DestFlags.Controller == UnitFlags.UnitController.Npc)
                {
                    npcName = evnt.DestName;
                    npcId = evnt.DestGuid;
                }
                
                if (!string.IsNullOrEmpty(npcName))
                {
                    state.InFight = true;
                    state.CurrentFight = new MonitoredFight()
                    {
                        BossName = npcName + " ID: " + ExtractHexPortionWithRegex(npcId),
                        BossPair = new Dictionary<string, string> { { npcName, ExtractHexPortionWithRegex(npcId) } },
                        FightStart = evnt.Timestamp,
                        MonsterID = new Dictionary<string, bool>() { { npcId, false } }
                    };
                    state.CalculatorFactory = new CalculatorFactory(_logger, _reporter, state, state.CurrentFight);
                    state.StartNewCombat();
                }
            }

            if (state.InFight)
            {
                state.InFight = state.CurrentFight.AddEvent(evnt, state);

                if (!state.InFight)
                    state.EndCombat();
            }

            return state.InFight;
        }
    }
}
